// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.for_CommandDescriptor.when_creating_from_command;

public class with_a_screen_matching_properties : Specification
{
    Command _command;
    Screen _screen;
    CommandDescriptor _result;

    void Establish()
    {
        _command = new Command(
            "PlaceOrder",
            "Places an order",
            [
                new Property("OrderId", "string"),
                new Property("Amount", "decimal")
            ],
            "OrderId");
        _screen = new Screen(
            "PlaceOrderScreen",
            "Screen to place an order",
            [
                new ScreenField("OrderId", "string", ScreenFieldType.TextInput, "Order ID"),
                new ScreenField("Amount", "decimal", ScreenFieldType.Number, "Amount")
            ]);
    }

    void Because() => _result = CommandDescriptor.FromCommand(_command, [], _screen, ConceptScope.Empty);

    [Fact] void should_set_field_type_on_order_id_property() => _result.Properties.First(p => p.Name == "OrderId").FieldType.ShouldEqual(ScreenFieldType.TextInput);
    [Fact] void should_set_label_on_order_id_property() => _result.Properties.First(p => p.Name == "OrderId").Label.ShouldEqual("Order ID");
    [Fact] void should_set_field_type_on_amount_property() => _result.Properties.First(p => p.Name == "Amount").FieldType.ShouldEqual(ScreenFieldType.Number);
    [Fact] void should_set_label_on_amount_property() => _result.Properties.First(p => p.Name == "Amount").Label.ShouldEqual("Amount");
}
