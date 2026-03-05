// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;
using Cratis.VerticalSlices.EventModelAdvisory.Rules;

namespace Cratis.VerticalSlices.for_PrimitivePropertyTypeRule.when_evaluating;

public class with_primitive_types_on_commands : Specification
{
    static Module[] _modules;
    List<EventModelRecommendation> _result;

    void Establish()
    {
        var eventType = new EventType("OrderPlaced", "An order was placed", [new Property("OrderId", "OrderId")]);
        var command = new Command(
            "PlaceOrder",
            "Places an order",
            [
                new Property("OrderId", "string"),
                new Property("Amount", "decimal")
            ],
            "OrderId");
        var slice = new VerticalSlice("PlaceOrder", VerticalSliceType.StateChange, null, null, [command], [], [eventType]);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because() => _result = new PrimitivePropertyTypeRule().Evaluate(_modules).ToList();

    [Fact] void should_return_two_recommendations() => _result.Count.ShouldEqual(2);
    [Fact] void should_reference_the_command_name() => _result[0].ArtifactName.ShouldEqual("PlaceOrder");
    [Fact] void should_mention_first_property() => _result[0].Message.ShouldContain("OrderId");
    [Fact] void should_mention_second_property() => _result[1].Message.ShouldContain("Amount");
}
