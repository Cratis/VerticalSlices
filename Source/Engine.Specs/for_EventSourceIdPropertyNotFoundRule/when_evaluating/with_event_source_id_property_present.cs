// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;
using Cratis.VerticalSlices.EventModelAdvisory.Rules;

namespace Cratis.VerticalSlices.for_EventSourceIdPropertyNotFoundRule.when_evaluating;

public class with_event_source_id_property_present : Specification
{
    static Module[] _modules;
    IEnumerable<EventModelRecommendation> _result;

    void Establish()
    {
        var command = new Command(
            "PlaceOrder",
            "Places an order",
            [
                new Property("OrderId", "Guid"),
                new Property("Amount", "decimal")
            ],
            "OrderId",
            EventSourceIdStrategy.Supplied);
        var slice = new VerticalSlice("PlaceOrder", VerticalSliceType.StateChange, null, null, [command], [], []);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because() => _result = new EventSourceIdPropertyNotFoundRule().Evaluate(_modules);

    [Fact] void should_return_no_recommendations() => _result.ShouldBeEmpty();
}
