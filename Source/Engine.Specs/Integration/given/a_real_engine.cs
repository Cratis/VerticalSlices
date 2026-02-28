// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Cratis.VerticalSlices.CodeGeneration;
using Cratis.VerticalSlices.CodeGeneration.Output;
using Cratis.VerticalSlices.CodeGeneration.SliceTypes;
using Microsoft.Extensions.Logging.Abstractions;

namespace Cratis.VerticalSlices.Integration.given;

/// <summary>
/// Base context that sets up a real <see cref="VerticalSlicesEngine"/> backed by all real
/// slice type generators and a <see cref="LocalFileSystemOutput"/> writing to a unique
/// temporary directory.  The directory and a minimal compilable project file are created
/// during Establish so that integration scenarios can run <c>dotnet build</c> against the
/// generated output.
/// </summary>
public class a_real_engine : Specification
{
    protected VerticalSlicesEngine _engine;
    protected LocalFileSystemOutput _output;
    protected string _outputDirectory;
    protected IEnumerable<GeneratedFile> _generatedFiles;
    protected int _buildExitCode;
    protected string _buildOutput = string.Empty;

    void Establish()
    {
        _outputDirectory = Path.Combine(Path.GetTempPath(), $"vs_integration_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_outputDirectory);

        _output = new LocalFileSystemOutput(
            _outputDirectory,
            NullLogger<LocalFileSystemOutput>.Instance);

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

        _engine = new VerticalSlicesEngine(
            codeGenerator,
            NullLogger<VerticalSlicesEngine>.Instance);

        WriteProjectFile();
    }

    void Destroy()
    {
        if (Directory.Exists(_outputDirectory))
        {
            Directory.Delete(_outputDirectory, recursive: true);
        }
    }

    void WriteProjectFile()
    {
        // NuGet.config — explicitly declares only the public nuget.org feed so the
        // temp build is self-contained and works on any machine without private-feed auth.
        var nugetConfig = new StringBuilder()
            .AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>")
            .AppendLine("<configuration>")
            .AppendLine("  <packageSources>")
            .AppendLine("    <clear />")
            .AppendLine("    <add key=\"nuget.org\" value=\"https://api.nuget.org/v3/index.json\" />")
            .AppendLine("  </packageSources>")
            .AppendLine("</configuration>")
            .ToString();

        File.WriteAllText(Path.Combine(_outputDirectory, "NuGet.config"), nugetConfig);

        var content = new StringBuilder()
            .AppendLine("<Project Sdk=\"Microsoft.NET.Sdk\">")
            .AppendLine("  <PropertyGroup>")
            .AppendLine("    <TargetFramework>net9.0</TargetFramework>")
            .AppendLine("    <Nullable>enable</Nullable>")
            .AppendLine("    <ImplicitUsings>enable</ImplicitUsings>")
            .AppendLine("    <TreatWarningsAsErrors>false</TreatWarningsAsErrors>")
            .AppendLine("    <NoWarn>SA0001;SA1600;CS1591;IDE0005;IDE0060;CS8019</NoWarn>")
            .AppendLine("  </PropertyGroup>")
            .AppendLine("  <ItemGroup>")
            .AppendLine("    <PackageReference Include=\"Cratis.Arc.Core\" Version=\"19.6.7\" />")
            .AppendLine("    <PackageReference Include=\"Cratis.Arc.MongoDB\" Version=\"19.6.7\" />")
            .AppendLine("    <PackageReference Include=\"Cratis.Chronicle\" Version=\"15.3.1\" />")
            .AppendLine("  </ItemGroup>")
            .AppendLine("</Project>")
            .ToString();

        File.WriteAllText(Path.Combine(_outputDirectory, "GeneratedCode.csproj"), content);

        // Global usings to bridge gaps between what the renderers generate and the actual package APIs.
        // IMongoCollection<T> and ISubject<T> are used directly in observable query files without being
        // listed in their generated using directives.
        var globalUsings = new StringBuilder()
            .AppendLine("global using MongoDB.Driver;")
            .AppendLine("global using System.Reactive.Subjects;")
            .ToString();

        File.WriteAllText(Path.Combine(_outputDirectory, "GlobalUsings.g.cs"), globalUsings);
    }

    /// <summary>
    /// Appends a <c>global using</c> directive for every namespace found in the already-previewed
    /// generated files to the <c>GlobalUsings.g.cs</c> that lives in the temp output directory.
    /// Call this after <see cref="VerticalSlicesEngine.Preview"/> and before <c>dotnet build</c> so
    /// that event types generated by one slice are visible to projections generated by another.
    /// </summary>
    protected void AddGlobalUsingsFromGeneratedFiles()
    {
        var namespaces = _generatedFiles
            .Select(f => ExtractNamespace(f.Content))
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

        File.AppendAllText(Path.Combine(_outputDirectory, "GlobalUsings.g.cs"), additions.ToString());
    }

    static string ExtractNamespace(string content)
    {
        var match = Regex.Match(content, @"^namespace\s+([\w.]+)\s*;", RegexOptions.Multiline);

        return match.Success ? match.Groups[1].Value : string.Empty;
    }

    /// <summary>Runs a dotnet command in the output directory and returns the exit code.</summary>
    /// <param name="arguments">The arguments to pass to the dotnet CLI.</param>
    /// <returns>The process exit code.</returns>
    protected async Task<int> RunDotnet(string arguments)
    {
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = arguments,
                WorkingDirectory = _outputDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            }
        };

        process.Start();

        var stdoutTask = process.StandardOutput.ReadToEndAsync();
        var stderrTask = process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        _buildOutput = (await stdoutTask) + (await stderrTask);

        return process.ExitCode;
    }
}
