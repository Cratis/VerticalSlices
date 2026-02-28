// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundReadModelRenderer.when_rendering;

public class with_subtract_from_property_mapping : given.a_context
{
    ModelBoundReadModelRenderer _renderer;
    ReadModelDescriptor _descriptor;
    string _projectionContent;

    void Establish()
    {
        _renderer = new ModelBoundReadModelRenderer();
        var mapping = new PropertyMapping("InventoryRemoved", PropertyMappingKind.Subtract, "Quantity");
        _descriptor = new ReadModelDescriptor(
            "InventoryItem",
            "An inventory item",
            [new ReadModelPropertyDescriptor("TotalQuantity", "int", IsKey: false, [mapping])],
            []);
    }

    void Because() => _projectionContent = _renderer.Render(_descriptor, _context)
        .Single(f => f.ArtifactPath.EndsWith("InventoryItem.cs")).Content;

    [Fact] void should_emit_subtract_from_attribute() => _projectionContent.ShouldContain("[SubtractFrom<InventoryRemoved>(");
    [Fact] void should_reference_event_property_by_nameof() => _projectionContent.ShouldContain("nameof(InventoryRemoved.Quantity)");
}
