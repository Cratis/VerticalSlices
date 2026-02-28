// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundReadModelRenderer.when_rendering;

public class with_count_property_mapping : given.a_context
{
    ModelBoundReadModelRenderer _renderer;
    ReadModelDescriptor _descriptor;
    string _projectionContent;

    void Establish()
    {
        _renderer = new ModelBoundReadModelRenderer();
        var mapping = new PropertyMapping("OrderPlaced", PropertyMappingKind.Count);
        _descriptor = new ReadModelDescriptor(
            "OrderSummary",
            "An order summary",
            [new ReadModelPropertyDescriptor("TotalOrders", "int", IsKey: false, [mapping])],
            []);
    }

    void Because() => _projectionContent = _renderer.Render(_descriptor, _context)
        .Single(f => f.ArtifactPath.EndsWith("OrderSummary.cs")).Content;

    [Fact] void should_emit_count_attribute() => _projectionContent.ShouldContain("[Count<OrderPlaced>]");
}
