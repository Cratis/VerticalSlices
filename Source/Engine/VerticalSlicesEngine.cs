// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.Chronicle;
using Cratis.VerticalSlices.CodeGeneration;
using Cratis.VerticalSlices.CodeGeneration.Descriptors;
using Cratis.VerticalSlices.CodeGeneration.Output;
using Cratis.VerticalSlices.CodeGeneration.Renderers;
using Cratis.VerticalSlices.EventModelAdvisory;
using Microsoft.Extensions.Logging;

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents an engine for processing vertical slices — generating code, writing output,
/// and registering artifacts with Chronicle.
/// </summary>
/// <param name="codeGenerator">The code generator for producing files from vertical slices.</param>
/// <param name="advisor">The event model advisor for analyzing modules before code generation.</param>
/// <param name="logger">The logger.</param>
/// <param name="loggerFactory">Factory used to create loggers for output and Chronicle implementations.</param>
public partial class VerticalSlicesEngine(
    IVerticalSliceCodeGenerator codeGenerator,
    IEventModelAdvisor advisor,
    ILogger<VerticalSlicesEngine> logger,
    ILoggerFactory loggerFactory) : IVerticalSlicesEngine
{
    /// <inheritdoc/>
    public async Task<VerticalSlicesResult> Process(
        IEnumerable<Module> modules,
        CodeGenerationOptions? options = null,
        CodeOutputOptions? outputOptions = null,
        ChronicleOptions? chronicleOptions = null,
        CancellationToken ct = default)
    {
        var moduleList = modules.ToList();
        var recommendations = advisor.Analyze(moduleList);
        if (recommendations.Any(r => r.Severity == EventModelRecommendationSeverity.Error))
        {
            return new VerticalSlicesResult(recommendations, []);
        }

        var resolvedOptions = options ?? new();
        var renderSet = ArtifactRenderSet.From(resolvedOptions);
        var collected = CollectFromModules(moduleList, renderSet, resolvedOptions);

        var resolvedOutput = ResolveOutput(outputOptions ?? new());
        var resolvedChronicle = ResolveChronicle(chronicleOptions ?? new());

        if (collected.Artifacts.Count > 0)
        {
            LogWritingFiles(collected.Artifacts.Count);
            await resolvedOutput.Write(collected.Artifacts, ct);
        }

        if (collected.EventDescriptors.Count > 0)
        {
            LogRegisteringEventTypes(collected.EventDescriptors.Count);
            await resolvedChronicle.RegisterEventTypes(collected.EventDescriptors, ct);
        }

        if (collected.ReadModelDescriptors.Count > 0)
        {
            LogRegisteringProjections(collected.ReadModelDescriptors.Count);
            await resolvedChronicle.RegisterProjections(collected.ReadModelDescriptors, ct);
            await resolvedChronicle.RegisterReadModelTypes(collected.ReadModelDescriptors, ct);
        }

        return new VerticalSlicesResult(recommendations, collected.Artifacts);
    }

    /// <inheritdoc/>
    public VerticalSlicesResult Preview(IEnumerable<Module> modules, CodeGenerationOptions? options = null)
    {
        var moduleList = modules.ToList();
        var recommendations = advisor.Analyze(moduleList);
        if (recommendations.Any(r => r.Severity == EventModelRecommendationSeverity.Error))
        {
            return new VerticalSlicesResult(recommendations, []);
        }

        var resolvedOptions = options ?? new();
        var renderSet = ArtifactRenderSet.From(resolvedOptions);
        var collected = CollectFromModules(moduleList, renderSet, resolvedOptions);
        return new VerticalSlicesResult(recommendations, collected.Artifacts);
    }

    /// <inheritdoc/>
    public IEnumerable<RenderedArtifact> PreviewSlice(
        VerticalSlice slice,
        string moduleName,
        FeaturePath featurePath,
        ConceptScope? conceptScope = null,
        CodeGenerationOptions? options = null)
    {
        var resolvedOptions = options ?? new();
        var renderSet = ArtifactRenderSet.From(resolvedOptions);
        var context = new CodeGenerationContext(moduleName, featurePath, slice.Name, resolvedOptions, conceptScope ?? ConceptScope.Empty);

        return codeGenerator.Generate(slice, context, renderSet);
    }

    CollectedArtifacts CollectFromModules(IEnumerable<Module> modules, ArtifactRenderSet renderSet, CodeGenerationOptions options)
    {
        var artifacts = new List<RenderedArtifact>();
        var eventDescriptors = new List<EventTypeDescriptor>();
        var readModelDescriptors = new List<ReadModelDescriptor>();

        foreach (var module in modules)
        {
            var moduleContext = new CodeGenerationContext(module.Name, FeaturePath.Empty, string.Empty, options);
            var moduleConceptScope = ConceptScope.Empty.With(module.Concepts, moduleContext.Namespace);

            foreach (var concept in module.Concepts)
            {
                artifacts.AddRange(renderSet.Concept.Render(ConceptDescriptor.FromConcept(concept), moduleContext));
            }

            foreach (var (moduleName, path, scope, feature) in module.Features.FlattenFeatures(module.Name, options.RootNamespace, moduleConceptScope))
            {
                LogProcessingFeature(feature.Name);

                var featureContext = new CodeGenerationContext(moduleName, path, string.Empty, options);

                foreach (var concept in feature.Concepts)
                {
                    artifacts.AddRange(renderSet.Concept.Render(ConceptDescriptor.FromConcept(concept), featureContext));
                }

                foreach (var slice in feature.VerticalSlices)
                {
                    var context = new CodeGenerationContext(moduleName, path, slice.Name, options, scope);
                    artifacts.AddRange(codeGenerator.Generate(slice, context, renderSet));

                    foreach (var eventType in slice.Events.Where(e => e.Kind == EventKind.Internal))
                    {
                        eventDescriptors.Add(EventTypeDescriptor.FromEventType(eventType, scope));
                    }

                    foreach (var readModel in slice.ReadModels)
                    {
                        readModelDescriptors.Add(ReadModelDescriptor.FromReadModel(readModel, slice.Events, slice.Screen));
                    }
                }
            }
        }

        return new CollectedArtifacts(artifacts, eventDescriptors, readModelDescriptors);
    }

    [LoggerMessage(LogLevel.Information, "Processing feature {FeatureName}")]
    partial void LogProcessingFeature(string featureName);

    [LoggerMessage(LogLevel.Information, "Writing {FileCount} generated files to output")]
    partial void LogWritingFiles(int fileCount);

    [LoggerMessage(LogLevel.Information, "Registering {Count} event types with Chronicle")]
    partial void LogRegisteringEventTypes(int count);

    [LoggerMessage(LogLevel.Information, "Registering {Count} projections and read model types with Chronicle")]
    partial void LogRegisteringProjections(int count);

    ICodeOutput ResolveOutput(CodeOutputOptions options) => options.Target switch
    {
        CodeOutputTarget.LocalFileSystem => new LocalFileSystemOutput(options.OutputRoot, loggerFactory.CreateLogger<LocalFileSystemOutput>()),
        CodeOutputTarget.InMemory => new InMemoryOutput(),
        _ => new NoOpCodeOutput()
    };

    IChronicleRegistration ResolveChronicle(ChronicleOptions options)
    {
        if (options.Target == ChronicleTarget.Http && options.HttpOptions is not null)
        {
#pragma warning disable CA2000 // HttpClient lifetime is scoped to the duration of the Process call
            return new ChronicleHttpRegistration(
                new HttpClient { BaseAddress = new Uri(options.HttpOptions.BaseUrl) },
                options.HttpOptions,
                loggerFactory.CreateLogger<ChronicleHttpRegistration>());
#pragma warning restore CA2000
        }

        return new NoOpChronicleRegistration();
    }
}
