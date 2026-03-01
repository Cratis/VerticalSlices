// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundReadModelRenderer.when_rendering;

public class without_description : given.a_context
{
    ModelBoundReadModelRenderer _renderer;
    ReadModelDescriptor _descriptor;
    string _projectionContent;

    void Establish()
    {
        _renderer = new ModelBoundReadModelRenderer();
        _descriptor = new ReadModelDescriptor(
            "OrderSummary",
            "",
            [new ReadModelPropertyDescriptor("OrderId", "string", IsKey: true, [])],
            []);
    }

    void Because() =>
        _projectionContent = _renderer.Render(_descriptor, _context)
            .Single(f => f.ArtifactPath.EndsWith("OrderSummary.cs"))
            .Content;

    [Fact] void should_not_emit_type_level_summary_comment() =>
        _projectionContent.Split("[ReadModel]")[0].ShouldNotContain("/// <summary>");
    [Fact] void should_still_emit_record_declaration() => _projectionContent.ShouldContain("public record OrderSummary(");
}
