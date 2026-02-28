// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundReadModelRenderer.when_rendering;

/// <summary>
/// A property with a FromEventSourceId mapping kind produces no attribute in the renderer
/// (the switch statement returns null for that kind). The property should still appear in the
/// projection record but without any mapping attribute on it.
/// </summary>
public class with_from_event_source_id_mapping : given.a_context
{
    ModelBoundReadModelRenderer _renderer;
    ReadModelDescriptor _descriptor;
    string _projectionContent;

    void Establish()
    {
        _renderer = new ModelBoundReadModelRenderer();

        var sourceIdMapping = new PropertyMapping("ItemUpdated", PropertyMappingKind.FromEventSourceId, EventPropertyName: "ItemId");

        _descriptor = new ReadModelDescriptor(
            "ItemView",
            "A view of an item",
            [
                new ReadModelPropertyDescriptor("ItemId", "string", IsKey: true, [sourceIdMapping])
            ],
            []);
    }

    void Because() => _projectionContent = _renderer.Render(_descriptor, _context)
        .Single(f => f.RelativePath.EndsWith("ItemView.cs")).Content;

    [Fact] void should_include_key_attribute() => _projectionContent.ShouldContain("[Key]");
    [Fact] void should_not_emit_any_from_event_source_id_attribute() =>
        _projectionContent.ShouldNotContain("[FromEventSourceId");
    [Fact] void should_still_include_the_property() => _projectionContent.ShouldContain("string ItemId");
}
