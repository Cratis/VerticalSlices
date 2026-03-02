// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;
using Cratis.VerticalSlices.EventModelAdvisory.Rules;

namespace Cratis.VerticalSlices.for_DuplicateEventTypeNameRule.when_evaluating;

public class with_unique_event_names : Specification
{
    static Module[] _modules;
    IEnumerable<EventModelRecommendation> _result;

    void Establish()
    {
        var event1 = new EventType("OrderPlaced", "An order was placed", [new Property("OrderId", "Guid")]);
        var event2 = new EventType("OrderShipped", "An order was shipped", [new Property("OrderId", "Guid")]);
        var command = new Command("PlaceOrder", "Places an order", [], "OrderId");
        var stateChange = new VerticalSlice("PlaceOrder", VerticalSliceType.StateChange, null, null, [command], [], [event1]);
        var stateView = new VerticalSlice("ViewOrders", VerticalSliceType.StateView, null, null, [], [], [event2]);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [stateChange, stateView])])];
    }

    void Because() => _result = new DuplicateEventTypeNameRule().Evaluate(_modules);

    [Fact] void should_return_no_recommendations() => _result.ShouldBeEmpty();
}
