// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.for_CommandDescriptor.when_creating_from_command;

public class without_a_screen : Specification
{
    Command _command;
    EventType _producedEvent;
    CommandDescriptor _result;

    void Establish()
    {
        _command = new Command("PlaceOrder", "Places an order", [new Property("OrderId", "string"), new Property("Amount", "decimal")], "OrderId");
        _producedEvent = new EventType("OrderPlaced", "An order was placed", []);
    }

    void Because() => _result = CommandDescriptor.FromCommand(_command, [_producedEvent], null, ConceptScope.Empty);

    [Fact] void should_map_name() => _result.Name.ShouldEqual("PlaceOrder");
    [Fact] void should_map_description() => _result.Description.ShouldEqual("Places an order");
    [Fact] void should_map_all_properties() => _result.Properties.Count().ShouldEqual(2);
    [Fact] void should_include_produced_event() => _result.ProducedEvents.Count().ShouldEqual(1);
    [Fact] void should_map_produced_event_name() => _result.ProducedEvents.First().EventType.Name.ShouldEqual("OrderPlaced");
    [Fact] void should_have_no_field_type_on_properties() => _result.Properties.All(p => p.FieldType is null).ShouldBeTrue();
    [Fact] void should_have_no_label_on_properties() => _result.Properties.All(p => p.Label is null).ShouldBeTrue();
}
