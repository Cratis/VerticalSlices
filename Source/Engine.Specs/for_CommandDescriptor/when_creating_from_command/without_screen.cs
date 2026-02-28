// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.for_CommandDescriptor.when_creating_from_command;

public class without_screen : Specification
{
    Command _command;
    CommandDescriptor _result;
    EventType _producedEvent;

    void Establish()
    {
        _producedEvent = new EventType("OrderPlaced", "An order was placed", []);
        _command = new Command("PlaceOrder", "Places an order", [new Property("OrderId", "Guid")], "OrderId");
    }

    void Because() => _result = CommandDescriptor.FromCommand(_command, [_producedEvent]);

    [Fact] void should_map_name() => _result.Name.ShouldEqual("PlaceOrder");
    [Fact] void should_map_description() => _result.Description.ShouldEqual("Places an order");
    [Fact] void should_map_property_name() => _result.Properties.First().Name.ShouldEqual("OrderId");
    [Fact] void should_map_property_type() => _result.Properties.First().Type.ShouldEqual("Guid");
    [Fact] void should_have_null_field_type_without_screen() => _result.Properties.First().FieldType.ShouldBeNull();
    [Fact] void should_have_null_label_without_screen() => _result.Properties.First().Label.ShouldBeNull();
    [Fact] void should_map_produced_events() => _result.ProducedEvents.First().Name.ShouldEqual("OrderPlaced");
}
