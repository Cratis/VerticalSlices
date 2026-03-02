// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;
using Cratis.VerticalSlices.EventModelAdvisory.Rules;

namespace Cratis.VerticalSlices.for_DuplicatePropertyNameRule.when_evaluating;

public class with_unique_property_names : Specification
{
    static Module[] _modules;
    IEnumerable<EventModelRecommendation> _result;

    void Establish()
    {
        var eventType = new EventType("OrderPlaced", "An order was placed",
        [
            new Property("OrderId", "Guid"),
            new Property("Amount", "decimal")
        ]);
        var command = new Command("PlaceOrder", "Places an order",
        [
            new Property("OrderId", "Guid"),
            new Property("Amount", "decimal")
        ], "OrderId");
        var slice = new VerticalSlice("PlaceOrder", VerticalSliceType.StateChange, null, null, [command], [], [eventType]);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because() => _result = new DuplicatePropertyNameRule().Evaluate(_modules);

    [Fact] void should_return_no_recommendations() => _result.ShouldBeEmpty();
}
