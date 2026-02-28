// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.for_EventTypeDescriptor.when_creating_from_event_type;

public class with_an_event_type : Specification
{
    EventType _eventType;
    EventTypeDescriptor _result;

    void Establish() => _eventType = new EventType(
        "OrderPlaced",
        "An order was placed",
        [new Property("OrderId", "Guid"), new Property("Amount", "decimal")]);

    void Because() => _result = EventTypeDescriptor.FromEventType(_eventType);

    [Fact] void should_map_name() => _result.Name.ShouldEqual("OrderPlaced");
    [Fact] void should_map_description() => _result.Description.ShouldEqual("An order was placed");
    [Fact] void should_map_all_properties() => _result.Properties.Count().ShouldEqual(2);
    [Fact] void should_map_first_property_name() => _result.Properties.First().Name.ShouldEqual("OrderId");
    [Fact] void should_map_second_property_name() => _result.Properties.ElementAt(1).Name.ShouldEqual("Amount");
}
