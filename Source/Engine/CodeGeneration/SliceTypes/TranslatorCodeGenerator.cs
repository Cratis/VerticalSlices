// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;
using Cratis.VerticalSlices.CodeGeneration.Renderers;

namespace Cratis.VerticalSlices.CodeGeneration.SliceTypes;

/// <summary>
/// Generates code for Translator slices. A Translator slice takes external events
/// and translates them into internal domain events.
/// Flow: External EventType(s) → translated internal EventType(s).
/// </summary>
public class TranslatorCodeGenerator : ISliceTypeCodeGenerator
{
    /// <inheritdoc/>
    public VerticalSliceType SliceType => VerticalSliceType.Translator;

    /// <inheritdoc/>
    public IEnumerable<GeneratedFile> Generate(VerticalSlice slice, CodeGenerationContext context, ArtifactRenderSet renderSet)
    {
        var files = new List<GeneratedFile>();

        foreach (var eventType in slice.Events)
        {
            var descriptor = EventTypeDescriptor.FromEventType(eventType);
            files.AddRange(renderSet.EventType.Render(descriptor, context));
        }

        foreach (var readModel in slice.ReadModels)
        {
            var descriptor = ReadModelDescriptor.FromReadModel(readModel, slice.Events, slice.Screen);
            files.AddRange(renderSet.ReadModel.Render(descriptor, context));
        }

        return files;
    }
}
