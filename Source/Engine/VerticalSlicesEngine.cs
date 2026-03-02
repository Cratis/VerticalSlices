// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.DependencyInjection;
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
/// <param name="outputResolver">Resolver for the output target. Resolves the appropriate code output based on configuration.</param>
/// <param name="chronicleResolver">Resolver for the Chronicle registration target. Resolves the appropriate registration strategy based on configuration.</param>
[Singleton]
public partial class VerticalSlicesEngine(
    IVerticalSliceCodeGenerator codeGenerator,
    IEventModelAdvisor advisor,
    ILogger<VerticalSlicesEngine> logger,
    ICodeOutputResolver outputResolver,
    IChronicleRegistrationResolver chronicleResolver) : IVerticalSlicesEngine
{
    /// <inheritdoc/>
    public async Task<VerticalSlicesResult> Process(
        IEnumerable<Module> modules,
        CodeGenerationOptions? options = null,
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

        var output = outputResolver.Resolve();
        if (collected.Artifacts.Count > 0)
        {
            LogWritingFiles(collected.Artifacts.Count);
            await output.Write(collected.Artifacts, ct);
        }

        var chronicle = chronicleResolver.Resolve();
        if (collected.EventDescriptors.Count > 0)
        {
            LogRegisteringEventTypes(collected.EventDescriptors.Count);
            await chronicle.RegisterEventTypes(collected.EventDescriptors, ct);
        }

        if (collected.ReadModelDescriptors.Count > 0)
        {
            LogRegisteringProjections(collected.ReadModelDescriptors.Count);
            await chronicle.RegisterProjections(collected.ReadModelDescriptors, ct);
            await chronicle.RegisterReadModelTypes(collected.ReadModelDescriptors, ct);
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
        return new VerticalSlicesResult(recommendations, CollectFromModules(moduleList, renderSet, resolvedOptions).Artifacts);
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
                var descriptor = ConceptDescriptor.FromConcept(concept);
                artifacts.AddRange(renderSet.Concept.Render(descriptor, moduleContext));
            }

            CollectFromFeatures(module.Features, renderSet, artifacts, eventDescriptors, readModelDescriptors, module.Name, FeaturePath.Empty, moduleConceptScope, options);
        }

        return new CollectedArtifacts(artifacts, eventDescriptors, readModelDescriptors);
    }

    void CollectFromFeatures(
        IEnumerable<Feature> features,
        ArtifactRenderSet renderSet,
        List<RenderedArtifact> artifacts,
        List<EventTypeDescriptor> eventDescriptors,
        List<ReadModelDescriptor> readModelDescriptors,
        string moduleName,
        FeaturePath featurePath,
        ConceptScope conceptScope,
        CodeGenerationOptions options)
    {
        foreach (var feature in features)
        {
            LogProcessingFeature(feature.Name);

            var currentFeaturePath = featurePath.Append(feature.Name);
            var featureContext = new CodeGenerationContext(moduleName, currentFeaturePath, string.Empty, options);
            var currentConceptScope = conceptScope.With(feature.Concepts, featureContext.Namespace);

            foreach (var concept in feature.Concepts)
            {
                var descriptor = ConceptDescriptor.FromConcept(concept);
                artifacts.AddRange(renderSet.Concept.Render(descriptor, featureContext));
            }

            foreach (var slice in feature.VerticalSlices)
            {
                var context = new CodeGenerationContext(moduleName, currentFeaturePath, slice.Name, options, currentConceptScope);
                artifacts.AddRange(codeGenerator.Generate(slice, context, renderSet));

                foreach (var eventType in slice.Events.Where(e => e.Kind == EventKind.Internal))
                {
                    eventDescriptors.Add(EventTypeDescriptor.FromEventType(eventType, currentConceptScope));
                }

                foreach (var readModel in slice.ReadModels)
                {
                    readModelDescriptors.Add(ReadModelDescriptor.FromReadModel(readModel, slice.Events, slice.Screen));
                }
            }

            if (feature.Features.Any())
            {
                CollectFromFeatures(feature.Features, renderSet, artifacts, eventDescriptors, readModelDescriptors, moduleName, currentFeaturePath, currentConceptScope, options);
            }
        }
    }

    [LoggerMessage(LogLevel.Information, "Processing feature {FeatureName}")]
    partial void LogProcessingFeature(string featureName);

    [LoggerMessage(LogLevel.Information, "Writing {FileCount} generated files to output")]
    partial void LogWritingFiles(int fileCount);

    [LoggerMessage(LogLevel.Information, "Registering {Count} event types with Chronicle")]
    partial void LogRegisteringEventTypes(int count);

    [LoggerMessage(LogLevel.Information, "Registering {Count} projections and read model types with Chronicle")]
    partial void LogRegisteringProjections(int count);
}
