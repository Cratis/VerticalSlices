// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.for_ReadModelDescriptor.when_creating_from_read_model;

/// <summary>
/// A StaticValue mapping sets the property to a constant value instead of reading from the event.
/// It must be preserved in the descriptor with Kind as StaticValue, with the source property name
/// in EventPropertyName (not ContextProperty).
/// </summary>
public class with_static_value_mapping_kind : Specification
{
    ReadModel _readModel;
    ReadModelDescriptor _result;

    void Establish()
    {
        var staticMapping = new EventPropertyMapping("OrderPlaced", EventPropertyMappingKind.StaticValue, "Active");

        _readModel = new ReadModel(
            "OrderSummary",
            "Summary of an order",
            [new ReadModelProperty("Status", "string", [staticMapping])]);
    }

    void Because() => _result = ReadModelDescriptor.FromReadModel(
        _readModel,
        [new EventType("OrderPlaced", "Order placed", [new Property("OrderId", "string")])]);

    [Fact] void should_preserve_static_value_kind() =>
        _result.Properties.First().Mappings.First().Kind.ShouldEqual(PropertyMappingKind.StaticValue);

    [Fact] void should_route_source_to_event_property_name() =>
        _result.Properties.First().Mappings.First().EventPropertyName.ShouldEqual("Active");

    [Fact] void should_have_null_context_property() =>
        _result.Properties.First().Mappings.First().ContextProperty.ShouldBeNull();
}
