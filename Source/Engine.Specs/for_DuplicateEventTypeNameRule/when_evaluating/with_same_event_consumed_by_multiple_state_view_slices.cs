// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;
using Cratis.VerticalSlices.EventModelAdvisory.Rules;

namespace Cratis.VerticalSlices.for_DuplicateEventTypeNameRule.when_evaluating;

public class with_same_event_consumed_by_multiple_state_view_slices : Specification
{
    static Module[] _modules;
    IEnumerable<EventModelRecommendation> _result;

    void Establish()
    {
        var sharedEvent = new EventType("OrderPlaced", "An order was placed", [new Property("OrderId", "Guid")]);
        var stateView1 = new VerticalSlice("OrderSummaryView", VerticalSliceType.StateView, null, null, [], [], [sharedEvent]);
        var stateView2 = new VerticalSlice("OrderDetailView", VerticalSliceType.StateView, null, null, [], [], [sharedEvent]);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [stateView1, stateView2])])];
    }

    void Because() => _result = new DuplicateEventTypeNameRule().Evaluate(_modules);

    [Fact] void should_return_no_recommendations() => _result.ShouldBeEmpty();
}
