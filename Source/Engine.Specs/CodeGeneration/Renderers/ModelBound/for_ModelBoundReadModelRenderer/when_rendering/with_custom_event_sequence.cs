// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundReadModelRenderer.when_rendering;

public class with_custom_event_sequence : given.a_context
{
    ModelBoundReadModelRenderer _renderer;
    ReadModelDescriptor _descriptor;
    string _projectionContent;

    void Establish()
    {
        _renderer = new ModelBoundReadModelRenderer();
        _descriptor = new ReadModelDescriptor(
            "ArchivedEmployee",
            "An archived employee projection",
            [new ReadModelPropertyDescriptor("Id", "string", IsKey: true, [])],
            [],
            EventSequence: "archive");
    }

    void Because() => _projectionContent = _renderer.Render(_descriptor, _context)
        .Single(f => f.ArtifactPath.EndsWith("ArchivedEmployee.cs")).Content;

    [Fact] void should_emit_from_event_sequence_attribute() => _projectionContent.ShouldContain("[FromEventSequence(\"archive\")]");
}
