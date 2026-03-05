// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.for_ReadModelDescriptor.when_creating_from_read_model;

public class without_screen : Specification
{
    ReadModel _readModel;
    EventType _referencedEvent;
    EventType _unreferencedEvent;
    ReadModelDescriptor _result;

    void Establish()
    {
        _referencedEvent = new EventType("OrderPlaced", "An order was placed", [new Property("OrderId", "string")]);
        _unreferencedEvent = new EventType("OrderShipped", "An order was shipped", []);

        var orderIdMapping = new EventPropertyMapping("OrderPlaced", EventPropertyMappingKind.Set, "OrderId");
        var idProperty = new ReadModelProperty("Id", "string", [orderIdMapping]);
        var nameProperty = new ReadModelProperty("Name", "string", []);

        _readModel = new ReadModel("OrderList", "List of orders", [idProperty, nameProperty]);
    }

    void Because() => _result = ReadModelDescriptor.FromReadModel(_readModel, [_referencedEvent, _unreferencedEvent]);

    [Fact] void should_map_name() => _result.Name.ShouldEqual("OrderList");
    [Fact] void should_map_description() => _result.Description.ShouldEqual("List of orders");
    [Fact] void should_mark_first_property_as_key() => _result.Properties.First().IsKey.ShouldBeTrue();
    [Fact] void should_not_mark_second_property_as_key() => _result.Properties.ElementAt(1).IsKey.ShouldBeFalse();
    [Fact] void should_include_only_referenced_event_in_source_events() => _result.SourceEvents.Count().ShouldEqual(1);
    [Fact] void should_include_the_referenced_event() => _result.SourceEvents.First().Name.ShouldEqual("OrderPlaced");
    [Fact] void should_map_property_mappings() => _result.Properties.First().Mappings.Count().ShouldEqual(1);
    [Fact] void should_map_property_mapping_event_type_name() => _result.Properties.First().Mappings.First().EventTypeName.ShouldEqual("OrderPlaced");
    [Fact] void should_map_property_mapping_kind() => _result.Properties.First().Mappings.First().Kind.ShouldEqual(PropertyMappingKind.Set);
    [Fact] void should_map_property_mapping_source_property() => _result.Properties.First().Mappings.First().EventPropertyName.ShouldEqual("OrderId");
}
