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
        IEnumerable<Feature> features,
        ICodeOutput? output = null,
        IChronicleRegistration? chronicle = null,
        ArtifactRenderSet? renderSet = null,
        CancellationToken ct = default)
    {
        renderSet ??= ArtifactRenderSet.ModelBound;
        var allFiles = new List<GeneratedFile>();
        var allEventDescriptors = new List<EventTypeDescriptor>();
        var allReadModelDescriptors = new List<ReadModelDescriptor>();

        CollectFromFeatures(features, renderSet, allFiles, allEventDescriptors, allReadModelDescriptors);

        if (output is not null && allFiles.Count > 0)
        {
            LogWritingFiles(allFiles.Count);
            await output.Write(allFiles, ct);
        }

        if (chronicle is not null)
        {
            if (allEventDescriptors.Count > 0)
            {
                LogRegisteringEventTypes(allEventDescriptors.Count);
                await chronicle.RegisterEventTypes(allEventDescriptors, ct);
            }

            if (allReadModelDescriptors.Count > 0)
            {
                LogRegisteringProjections(allReadModelDescriptors.Count);
                await chronicle.RegisterProjections(allReadModelDescriptors, ct);
                await chronicle.RegisterReadModelTypes(allReadModelDescriptors, ct);
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<GeneratedFile> Preview(IEnumerable<Feature> features, ArtifactRenderSet? renderSet = null)
    {
        renderSet ??= ArtifactRenderSet.ModelBound;
        var allFiles = new List<GeneratedFile>();

        CollectFromFeatures(features, renderSet, allFiles, [], []);

        return allFiles;
    }

    void CollectFromFeatures(
        IEnumerable<Feature> features,
        ArtifactRenderSet renderSet,
        List<GeneratedFile> files,
        List<EventTypeDescriptor> eventDescriptors,
        List<ReadModelDescriptor> readModelDescriptors,
        List<string>? parentFeatures = null)
    {
        parentFeatures ??= [];

        foreach (var feature in features)
        {
            LogProcessingFeature(feature.Name);

            // Generate concept files at the feature level
            foreach (var concept in feature.Concepts)
            {
                var conceptContext = new CodeGenerationContext(feature.Name, string.Empty, parentFeatures);
                var descriptor = ConceptDescriptor.FromConcept(concept);
                files.AddRange(renderSet.Concept.Render(descriptor, conceptContext));
            }

            foreach (var slice in feature.VerticalSlices)
            {
                var context = new CodeGenerationContext(feature.Name, slice.Name, parentFeatures);
                var generated = codeGenerator.Generate(slice, context, renderSet);
                files.AddRange(generated);

                // Collect descriptors for Chronicle registration
                foreach (var eventType in slice.Events)
                {
                    eventDescriptors.Add(EventTypeDescriptor.FromEventType(eventType));
                }

                foreach (var readModel in slice.ReadModels)
                {
                    readModelDescriptors.Add(ReadModelDescriptor.FromReadModel(readModel, slice.Screen));
                }
            }

            if (feature.Features.Any())
            {
                var subPath = new List<string>(parentFeatures) { feature.Name };
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
