// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.DependencyInjection;
using Cratis.VerticalSlices.CodeGeneration.Descriptors;
using Cratis.VerticalSlices.CodeGeneration.Renderers;

namespace Cratis.VerticalSlices.CodeGeneration.SliceTypes;

/// <summary>
/// Generates code for StateView slices. A StateView slice projects events into read models
/// that are displayed on a screen. Each read model carries its own event dependencies
/// and produces both a projection and an Observable query.
/// Flow: EventType(s) → ReadModel (projection + query) → Screen.
/// </summary>
[Singleton]
public class StateViewCodeGenerator : ISliceTypeCodeGenerator
{
    /// <inheritdoc/>
    public VerticalSliceType SliceType => VerticalSliceType.StateView;

    /// <inheritdoc/>
    public IEnumerable<RenderedArtifact> Generate(VerticalSlice slice, CodeGenerationContext context, ArtifactRenderSet renderSet)
    {
        var artifacts = new List<RenderedArtifact>();

        foreach (var readModel in slice.ReadModels)
        {
            var descriptor = ReadModelDescriptor.FromReadModel(readModel, slice.Events, slice.Screen);
            artifacts.AddRange(renderSet.ReadModel.Render(descriptor, context));
        }

        return artifacts;
    }
}
