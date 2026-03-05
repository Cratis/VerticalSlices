// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Cratis.VerticalSlices.CodeGeneration;
using Cratis.VerticalSlices.CodeGeneration.Output;
using Cratis.VerticalSlices.CodeGeneration.SliceTypes;
using Cratis.VerticalSlices.EventModelAdvisory;
using Microsoft.Extensions.Logging.Abstractions;

namespace Cratis.VerticalSlices.Integration.given;

/// <summary>
/// xUnit class fixture that sets up a real <see cref="VerticalSlicesEngine"/> backed by all real
/// slice type generators and a <see cref="LocalFileSystemOutput"/> writing to a unique
/// temporary directory. The setup runs once per test class, not once per test.
/// </summary>
public partial class EngineFixture : IAsyncLifetime
{
    /// <summary>
    /// Gets the engine instance.
    /// </summary>
    public VerticalSlicesEngine Engine { get; private set; } = null!;

    /// <summary>
    /// Gets the output directory where generated files are written.
    /// </summary>
    public string OutputDirectory { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the code output options used when calling <see cref="VerticalSlicesEngine.Process"/>.
    /// </summary>
    public CodeOutputOptions OutputOptions { get; private set; } = new();

    /// <summary>
    /// Gets the generated files from the last <see cref="VerticalSlicesEngine.Preview"/> call.
    /// </summary>
    public IEnumerable<RenderedArtifact> GeneratedFiles { get; private set; } = [];

    /// <summary>
    /// Gets the exit code from the last <c>dotnet build</c> invocation.
    /// </summary>
    public int BuildExitCode { get; private set; }

    /// <summary>
    /// Gets the combined stdout and stderr from the last <c>dotnet build</c> invocation.
    /// </summary>
    public string BuildOutput { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the modules that were processed by the engine.
    /// </summary>
    public IEnumerable<Module> Modules { get; private set; } = [];

    /// <inheritdoc/>
    public virtual async Task InitializeAsync()
    {
        OutputDirectory = Path.Combine(Path.GetTempPath(), $"vs_integration_{Guid.NewGuid():N}");
        Directory.CreateDirectory(OutputDirectory);

        OutputOptions = new CodeOutputOptions { Target = CodeOutputTarget.LocalFileSystem, OutputRoot = OutputDirectory };

        var sliceGenerators = new ISliceTypeCodeGenerator[]
        {
            new StateChangeCodeGenerator(),
            new StateViewCodeGenerator(),
            new AutomationCodeGenerator(),
            new TranslatorCodeGenerator()
        };

        var codeGenerator = new VerticalSliceCodeGenerator(
            sliceGenerators,
            NullLogger<VerticalSliceCodeGenerator>.Instance);

        Engine = new VerticalSlicesEngine(
            codeGenerator,
            new EventModelAdvisor(new InstancesOf<IEventModelRule>(AppTypes.Instance, new ActivatorServiceProvider())),
            NullLogger<VerticalSlicesEngine>.Instance,
            NullLoggerFactory.Instance);

        WriteProjectFile();
        await RunDotnet("add package Cratis");
        await RunDotnet("add package Cratis.Arc.MongoDB");
    }

    /// <inheritdoc/>
    public Task DisposeAsync()
    {
        if (Directory.Exists(OutputDirectory))
        {
            Directory.Delete(OutputDirectory, recursive: true);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Processes the given modules through the engine, writes output, generates global usings,
    /// and runs <c>dotnet build</c>.
    /// </summary>
    /// <param name="modules">The modules to process.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task ProcessAndBuild(IEnumerable<Module> modules)
    {
        Modules = modules;
        await Engine.Process(modules, outputOptions: OutputOptions);
        GeneratedFiles = Engine.Preview(modules).Artifacts;
        AddGlobalUsingsFromRenderedArtifacts();
        BuildExitCode = await RunDotnet("build");
    }

    void WriteProjectFile()
    {
        var nugetConfig = new StringBuilder()
            .AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>")
            .AppendLine("<configuration>")
            .AppendLine("  <packageSources>")
            .AppendLine("    <clear />")
            .AppendLine("    <add key=\"nuget.org\" value=\"https://api.nuget.org/v3/index.json\" />")
            .AppendLine("  </packageSources>")
            .AppendLine("</configuration>")
            .ToString();

        File.WriteAllText(Path.Combine(OutputDirectory, "NuGet.config"), nugetConfig);

        var content = new StringBuilder()
            .AppendLine("<Project Sdk=\"Microsoft.NET.Sdk\">")
            .AppendLine("  <PropertyGroup>")
            .AppendLine("    <TargetFramework>net10.0</TargetFramework>")
            .AppendLine("    <Nullable>enable</Nullable>")
            .AppendLine("    <ImplicitUsings>enable</ImplicitUsings>")
            .AppendLine("    <TreatWarningsAsErrors>false</TreatWarningsAsErrors>")
            .AppendLine("    <NoWarn>SA0001;SA1600;CS1591;IDE0005;IDE0060;CS8019</NoWarn>")
            .AppendLine("  </PropertyGroup>")
            .AppendLine("</Project>")
            .ToString();

        File.WriteAllText(Path.Combine(OutputDirectory, "GeneratedCode.csproj"), content);
        File.WriteAllText(Path.Combine(OutputDirectory, "GlobalUsings.g.cs"), string.Empty);
    }

    void AddGlobalUsingsFromRenderedArtifacts()
    {
        var namespaces = GeneratedFiles
            .Select(a => ExtractNamespace(a.Content))
            .Where(ns => !string.IsNullOrWhiteSpace(ns))
            .Distinct()
            .ToList();

        if (namespaces.Count == 0)
        {
            return;
        }

        var additions = new StringBuilder();

        foreach (var ns in namespaces)
        {
            additions.AppendLine($"global using {ns};");
        }

        File.AppendAllText(Path.Combine(OutputDirectory, "GlobalUsings.g.cs"), additions.ToString());
    }

    static string ExtractNamespace(string content)
    {
        var match = NamespaceRegex().Match(content);

        return match.Success ? match.Groups["ns"].Value : string.Empty;
    }

#pragma warning disable MA0009
    [GeneratedRegex(@"^namespace\s+(?<ns>[\w.]+)\s*;", RegexOptions.Multiline | RegexOptions.ExplicitCapture)]
    private static partial Regex NamespaceRegex();
#pragma warning restore MA0009

    /// <summary>
    /// Runs a dotnet command in the output directory and returns the exit code.
    /// </summary>
    /// <param name="arguments">The arguments to pass to the dotnet CLI.</param>
    /// <returns>The process exit code.</returns>
    async Task<int> RunDotnet(string arguments)
    {
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = arguments,
                WorkingDirectory = OutputDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            }
        };

        process.Start();

        var stdoutTask = process.StandardOutput.ReadToEndAsync();
        var stderrTask = process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        BuildOutput = (await stdoutTask) + (await stderrTask);

        return process.ExitCode;
    }
}
