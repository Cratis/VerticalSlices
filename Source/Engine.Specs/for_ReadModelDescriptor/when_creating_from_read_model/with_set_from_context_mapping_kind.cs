// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.for_ReadModelDescriptor.when_creating_from_read_model;

/// <summary>
/// A SetFromContext mapping reads from EventContext metadata (e.g. Occurred timestamp).
/// FromReadModel must route the SourcePropertyName into ContextProperty — not EventPropertyName —
/// so that the renderer can emit [SetFromContext&lt;T&gt;(nameof(EventContext.X))].
/// </summary>
public class with_set_from_context_mapping_kind : Specification
{
    ReadModel _readModel;
    ReadModelDescriptor _result;

    void Establish()
    {
        var contextMapping = new EventPropertyMapping("OrderFulfilled", EventPropertyMappingKind.SetFromContext, "Occurred");

        _readModel = new ReadModel(
            "OrderAudit",
            "Audit trail for orders",
            [
                new ReadModelProperty("OrderId", "string", [new EventPropertyMapping("OrderFulfilled", EventPropertyMappingKind.Set, "OrderId")]),
                new ReadModelProperty("FulfilledAt", "DateTimeOffset", [contextMapping])
            ]);
    }

    void Because() => _result = ReadModelDescriptor.FromReadModel(
        _readModel,
        [new EventType("OrderFulfilled", "Order was fulfilled", [new Property("OrderId", "string")])]);

    [Fact] void should_preserve_set_from_context_kind() =>
        _result.Properties.ElementAt(1).Mappings.First().Kind.ShouldEqual(PropertyMappingKind.SetFromContext);

    [Fact] void should_put_source_property_into_context_property() =>
        _result.Properties.ElementAt(1).Mappings.First().ContextProperty.ShouldEqual("Occurred");

    [Fact] void should_have_null_event_property_name() =>
        _result.Properties.ElementAt(1).Mappings.First().EventPropertyName.ShouldBeNull();
}
