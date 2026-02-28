// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.for_EventTypeDescriptor.when_creating_from_event_type;

public class with_no_properties : Specification
{
    EventType _eventType;
    EventTypeDescriptor _result;

    void Establish() => _eventType = new EventType("OrderCancelled", "An order was cancelled", []);

    void Because() => _result = EventTypeDescriptor.FromEventType(_eventType);

    [Fact] void should_map_name() => _result.Name.ShouldEqual("OrderCancelled");
    [Fact] void should_map_description() => _result.Description.ShouldEqual("An order was cancelled");
    [Fact] void should_have_no_properties() => _result.Properties.ShouldBeEmpty();
}
