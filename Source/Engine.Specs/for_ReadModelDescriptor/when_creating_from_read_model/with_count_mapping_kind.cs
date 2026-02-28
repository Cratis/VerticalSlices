// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.for_ReadModelDescriptor.when_creating_from_read_model;

/// <summary>
/// A Count mapping kind means "count how many times event X fires". There is no source
/// property on the event to read — the descriptor must preserve Kind as Count.
/// </summary>
public class with_count_mapping_kind : Specification
{
    ReadModel _readModel;
    ReadModelDescriptor _result;

    void Establish()
    {
        var countMapping = new EventPropertyMapping("OrderPlaced", EventPropertyMappingKind.Count);

        _readModel = new ReadModel(
            "CustomerOrderStats",
            "Order statistics per customer",
            [
                new ReadModelProperty("CustomerId", "string", [new EventPropertyMapping("OrderPlaced", EventPropertyMappingKind.Set, "CustomerId")]),
                new ReadModelProperty("TotalOrders", "int", [countMapping])
            ]);
    }

    void Because() => _result = ReadModelDescriptor.FromReadModel(
        _readModel,
        [new EventType("OrderPlaced", "Order was placed", [new Property("CustomerId", "string"), new Property("OrderId", "string")])]);

    [Fact] void should_preserve_count_kind_on_total_orders_property() =>
        _result.Properties.ElementAt(1).Mappings.First().Kind.ShouldEqual(PropertyMappingKind.Count);

    [Fact] void should_allow_null_event_property_name_for_count() =>
        _result.Properties.ElementAt(1).Mappings.First().EventPropertyName.ShouldBeNull();
}
