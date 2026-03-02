// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;
using Cratis.VerticalSlices.EventModelAdvisory.Rules;

namespace Cratis.VerticalSlices.for_EventModelAdvisor.when_analyzing;

public class with_specific_rules_only : Specification
{
    static Module[] _modules;
    IReadOnlyList<EventModelRecommendation> _result;

    void Establish()
    {
        // Event has no read model consuming it — OrphanedEventRule should fire.
        var eventType = new EventType("OrderPlaced", "An order was placed", [new Property("OrderId", "Guid")]);
        var command = new Command("PlaceOrder", "Places an order", [], "OrderId");
        var slice = new VerticalSlice("PlaceOrder", VerticalSliceType.StateChange, null, null, [command], [], [eventType]);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because() => _result = new EventModelAdvisor().Analyze(_modules, [new OrphanedEventRule()]);

    [Fact] void should_return_only_matching_recommendations() => _result.Count.ShouldEqual(1);
    [Fact] void should_be_from_the_specified_rule() => _result[0].Category.ShouldEqual(EventModelRecommendationCategory.Coverage);
}

