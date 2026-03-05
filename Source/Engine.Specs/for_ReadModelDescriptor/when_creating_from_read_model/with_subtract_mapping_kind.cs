// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.for_ReadModelDescriptor.when_creating_from_read_model;

/// <summary>
/// A Subtract mapping kind reduces a numeric property from an event onto the read model total.
/// The source property name must be preserved in EventPropertyName, not ContextProperty.
/// </summary>
public class with_subtract_mapping_kind : Specification
{
    ReadModel _readModel;
    ReadModelDescriptor _result;

    void Establish()
    {
        var subtractMapping = new EventPropertyMapping("ItemRemoved", EventPropertyMappingKind.Subtract, "Quantity");

        _readModel = new ReadModel(
            "CartTotals",
            "Cart totals for a customer",
            [new ReadModelProperty("TotalQuantity", "int", [subtractMapping])]);
    }

    void Because() => _result = ReadModelDescriptor.FromReadModel(
        _readModel,
        [new EventType("ItemRemoved", "Item removed from cart", [new Property("Quantity", "int")])]);

    [Fact] void should_preserve_subtract_kind() =>
        _result.Properties.First().Mappings.First().Kind.ShouldEqual(PropertyMappingKind.Subtract);

    [Fact] void should_route_source_property_to_event_property_name() =>
        _result.Properties.First().Mappings.First().EventPropertyName.ShouldEqual("Quantity");

    [Fact] void should_have_null_context_property() =>
        _result.Properties.First().Mappings.First().ContextProperty.ShouldBeNull();
}
