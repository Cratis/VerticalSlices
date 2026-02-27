// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.Chronicle;
using Cratis.VerticalSlices.CodeGeneration;
using Cratis.VerticalSlices.CodeGeneration.Descriptors;
using Cratis.VerticalSlices.CodeGeneration.Output;
using Cratis.VerticalSlices.CodeGeneration.Renderers;
using Microsoft.Extensions.Logging;

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents an engine for processing vertical slices — generating code, writing output,
/// and registering artifacts with Chronicle.
/// </summary>
/// <param name="codeGenerator">The code generator for producing files from vertical slices.</param>
/// <param name="logger">The logger.</param>
public partial class VerticalSlicesEngine(
    IVerticalSliceCodeGenerator codeGenerator,
    ILogger<VerticalSlicesEngine> logger) : IVerticalSlicesEngine
{
    /// <inheritdoc/>
    public async Task Process(
        IEnumerable<Module> modules,
        ICodeOutput? output = null,
        IChronicleRegistration? chronicle = null,
        ArtifactRenderSet? renderSet = null,
        CancellationToken ct = default)
    {
        renderSet ??= ArtifactRenderSet.ModelBound;
        var collected = CollectFromModules(modules, renderSet);

        if (output is not null && collected.Files.Count > 0)
        {
            LogWritingFiles(collected.Files.Count);
            await output.Write(collected.Files, ct);
        }

        if (chronicle is not null)
        {
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
        }
    }

    /// <inheritdoc/>
    public IEnumerable<GeneratedFile> Preview(IEnumerable<Module> modules, ArtifactRenderSet? renderSet = null)
    {
        renderSet ??= ArtifactRenderSet.ModelBound;
        return CollectFromModules(modules, renderSet).Files;
    }

    CollectedArtifacts CollectFromModules(IEnumerable<Module> modules, ArtifactRenderSet renderSet)
    {
        SliceValidator.Validate(modules);

        var files = new List<GeneratedFile>();
        var eventDescriptors = new List<EventTypeDescriptor>();
        var readModelDescriptors = new List<ReadModelDescriptor>();

        foreach (var module in modules)
        {
            foreach (var concept in module.Concepts)
            {
                var context = new CodeGenerationContext(module.Name, string.Empty, []);
                var descriptor = ConceptDescriptor.FromConcept(concept);
                files.AddRange(renderSet.Concept.Render(descriptor, context));
            }

            CollectFromFeatures(module.Features, renderSet, files, eventDescriptors, readModelDescriptors, [module.Name]);
        }

        return new CollectedArtifacts(files, eventDescriptors, readModelDescriptors);
    }

    void CollectFromFeatures(
        IEnumerable<Feature> features,
        ArtifactRenderSet renderSet,
        List<GeneratedFile> files,
        List<EventTypeDescriptor> eventDescriptors,
        List<ReadModelDescriptor> readModelDescriptors,
        IReadOnlyList<string> parentPath)
    {
        foreach (var feature in features)
        {
            LogProcessingFeature(feature.Name);

            foreach (var concept in feature.Concepts)
            {
                var conceptContext = new CodeGenerationContext(feature.Name, string.Empty, parentPath);
                var descriptor = ConceptDescriptor.FromConcept(concept);
                files.AddRange(renderSet.Concept.Render(descriptor, conceptContext));
            }

            foreach (var slice in feature.VerticalSlices)
            {
                var context = new CodeGenerationContext(feature.Name, slice.Name, parentPath);
                files.AddRange(codeGenerator.Generate(slice, context, renderSet));

                foreach (var eventType in slice.Events.Where(e => e.Kind == EventKind.Internal))
                {
                    eventDescriptors.Add(EventTypeDescriptor.FromEventType(eventType));
                }

                foreach (var readModel in slice.ReadModels)
                {
                    readModelDescriptors.Add(ReadModelDescriptor.FromReadModel(readModel, slice.Events, slice.Screen));
                }
            }

            if (feature.Features.Any())
            {
                var subPath = new List<string>(parentPath) { feature.Name };
                CollectFromFeatures(feature.Features, renderSet, files, eventDescriptors, readModelDescriptors, subPath);
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
