// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.for_ReadModelDescriptor.when_creating_from_read_model;

public class with_a_screen_having_matching_field : Specification
{
    ReadModel _readModel;
    Screen _screen;
    ReadModelDescriptor _result;

    void Establish()
    {
        var idProperty = new ReadModelProperty("Id", "string", []);
        _readModel = new ReadModel("OrderList", "List of orders", [idProperty]);
        _screen = new Screen(
            "OrderListScreen",
            "Screen showing orders",
            [new ScreenField("Id", "string", ScreenFieldType.TextInput, "Order ID")]);
    }

    void Because() => _result = ReadModelDescriptor.FromReadModel(_readModel, [], _screen);

    [Fact] void should_set_field_type_from_screen() => _result.Properties.First().FieldType.ShouldEqual(ScreenFieldType.TextInput);
    [Fact] void should_set_label_from_screen() => _result.Properties.First().Label.ShouldEqual("Order ID");
}
