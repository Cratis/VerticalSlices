// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.for_ReadModelDescriptor.when_creating_from_read_model;

/// <summary>
/// A FromEventSourceId mapping copies the event identity (source id) onto the property.
/// It must be preserved with Kind as FromEventSourceId and source property name in EventPropertyName.
/// </summary>
public class with_from_event_source_id_mapping_kind : Specification
{
    ReadModel _readModel;
    ReadModelDescriptor _result;

    void Establish()
    {
        var sourceIdMapping = new EventPropertyMapping("ItemUpdated", EventPropertyMappingKind.FromEventSourceId, "ItemId");

        _readModel = new ReadModel(
            "ItemRecord",
            "Record of an item",
            [new ReadModelProperty("ItemId", "string", [sourceIdMapping])]);
    }

    void Because() => _result = ReadModelDescriptor.FromReadModel(
        _readModel,
        [new EventType("ItemUpdated", "Item was updated", [new Property("ItemId", "string")])]);

    [Fact] void should_preserve_from_event_source_id_kind() =>
        _result.Properties.First().Mappings.First().Kind.ShouldEqual(PropertyMappingKind.FromEventSourceId);

    [Fact] void should_route_source_to_event_property_name() =>
        _result.Properties.First().Mappings.First().EventPropertyName.ShouldEqual("ItemId");

    [Fact] void should_have_null_context_property() =>
        _result.Properties.First().Mappings.First().ContextProperty.ShouldBeNull();
}
