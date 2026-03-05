// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundReadModelRenderer.when_rendering;

public class with_not_rewindable_projection : given.a_context
{
    ModelBoundReadModelRenderer _renderer;
    ReadModelDescriptor _descriptor;
    string _projectionContent;

    void Establish()
    {
        _renderer = new ModelBoundReadModelRenderer();
        _descriptor = new ReadModelDescriptor(
            "AuditLog",
            "An append-only audit log projection",
            [new ReadModelPropertyDescriptor("Id", "string", IsKey: true, [])],
            [],
            IsNotRewindable: true);
    }

    void Because() => _projectionContent = _renderer.Render(_descriptor, _context)
        .Single(f => f.ArtifactPath.EndsWith("AuditLog.cs")).Content;

    [Fact] void should_emit_not_rewindable_attribute() => _projectionContent.ShouldContain("[NotRewindable]");
}
