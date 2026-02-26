// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;
using Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers;

/// <summary>
/// Bundles together the renderers for all artifact types, allowing slice type generators
/// to produce code using a specific variation (e.g., model-bound, controller-based).
/// </summary>
/// <param name="EventType">The renderer for event type descriptors.</param>
/// <param name="Command">The renderer for command descriptors.</param>
/// <param name="ReadModel">The renderer for read model / projection descriptors.</param>
/// <param name="Concept">The renderer for concept descriptors.</param>
public record ArtifactRenderSet(
    IArtifactRenderer<EventTypeDescriptor> EventType,
    IArtifactRenderer<CommandDescriptor> Command,
    IArtifactRenderer<ReadModelDescriptor> ReadModel,
    IArtifactRenderer<ConceptDescriptor> Concept)
{
    /// <summary>
    /// Gets the default render set using model-bound renderers for all artifact types.
    /// </summary>
    public static ArtifactRenderSet ModelBound { get; } = new(
        new ModelBoundEventTypeRenderer(),
        new ModelBoundCommandRenderer(),
        new ModelBoundReadModelRenderer(),
        new ModelBoundConceptRenderer());
}
