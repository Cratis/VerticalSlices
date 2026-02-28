// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundReadModelRenderer.when_rendering;

public class with_description : given.a_context
{
    ModelBoundReadModelRenderer _renderer;
    ReadModelDescriptor _descriptor;
    string _projectionContent;

    void Establish()
    {
        _renderer = new ModelBoundReadModelRenderer();
        _descriptor = new ReadModelDescriptor(
            "OrderSummary",
            "A summary view of an order",
            [new ReadModelPropertyDescriptor("OrderId", "string", IsKey: true, [])],
            []);
    }

    void Because() =>
        _projectionContent = _renderer.Render(_descriptor, _context)
            .Single(f => f.RelativePath.EndsWith("OrderSummary.cs"))
            .Content;

    [Fact] void should_emit_xml_summary_start() => _projectionContent.ShouldContain("/// <summary>");
    [Fact] void should_emit_description_text() => _projectionContent.ShouldContain("A summary view of an order");
    [Fact] void should_emit_xml_summary_end() => _projectionContent.ShouldContain("/// </summary>");
}
