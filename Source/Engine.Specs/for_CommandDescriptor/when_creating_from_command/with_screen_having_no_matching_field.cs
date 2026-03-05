// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.for_CommandDescriptor.when_creating_from_command;

public class with_screen_having_no_matching_field : Specification
{
    Command _command;
    Screen _screen;
    CommandDescriptor _result;

    void Establish()
    {
        _command = new Command("PlaceOrder", "Places an order", [new Property("CustomerId", "Guid")], "CustomerId");
        _screen = new Screen(
            "PlaceOrderScreen",
            "Screen for placing orders",
            [new ScreenField("OrderId", "Guid", ScreenFieldType.TextInput, "Order ID")]);
    }

    void Because() => _result = CommandDescriptor.FromCommand(_command, [], _screen, ConceptScope.Empty);

    [Fact] void should_have_null_field_type_when_no_match() => _result.Properties.First().FieldType.ShouldBeNull();
    [Fact] void should_have_null_label_when_no_match() => _result.Properties.First().Label.ShouldBeNull();
}
