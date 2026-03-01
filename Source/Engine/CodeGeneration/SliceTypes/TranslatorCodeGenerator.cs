// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;
using Cratis.VerticalSlices.CodeGeneration.Renderers;

namespace Cratis.VerticalSlices.CodeGeneration.SliceTypes;

/// <summary>
/// Generates code for Translator slices. A Translator slice consumes external events
/// and translates them into internal domain events.
/// Only <see cref="EventKind.Internal"/> events (the outputs) receive generated code and
/// Chronicle registration. <see cref="EventKind.External"/> events (the inputs) are
/// structural documentation only.
/// Flow: External EventType(s) → translated internal EventType(s).
/// </summary>
public class TranslatorCodeGenerator : ISliceTypeCodeGenerator
{
    /// <inheritdoc/>
    public VerticalSliceType SliceType => VerticalSliceType.Translator;

    /// <inheritdoc/>
    public IEnumerable<RenderedArtifact> Generate(VerticalSlice slice, CodeGenerationContext context, ArtifactRenderSet renderSet)
    {
        var artifacts = new List<RenderedArtifact>();

        foreach (var eventType in slice.Events.Where(e => e.Kind == EventKind.Internal))
        {
            var descriptor = EventTypeDescriptor.FromEventType(eventType, context.Concepts);
            artifacts.AddRange(renderSet.EventType.Render(descriptor, context));
        }

        foreach (var readModel in slice.ReadModels)
        {
            var descriptor = ReadModelDescriptor.FromReadModel(readModel, slice.Events, slice.Screen);
            artifacts.AddRange(renderSet.ReadModel.Render(descriptor, context));
        }

        return artifacts;
    }
}
