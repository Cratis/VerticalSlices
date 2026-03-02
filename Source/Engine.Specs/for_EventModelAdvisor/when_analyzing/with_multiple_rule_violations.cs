// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;

namespace Cratis.VerticalSlices.for_EventModelAdvisor.when_analyzing;

public class with_multiple_rule_violations : Specification
{
    static Module[] _modules;
    IReadOnlyList<EventModelRecommendation> _result;

    void Establish()
    {
        var emptyEvent = new EventType("PlaceOrder", "Bad naming and no properties", []);
        var command = new Command("PlaceOrder", "Places an order", [new Property("OrderId", "string")], "OrderId");
        var slice = new VerticalSlice("PlaceOrder", VerticalSliceType.StateChange, null, null, [command], [], [emptyEvent]);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because() => _result = new EventModelAdvisor().Analyze(_modules);

    [Fact] void should_return_recommendations() => _result.ShouldNotBeEmpty();
    [Fact] void should_order_by_severity_descending() => _result.First().Severity.ShouldBeGreaterThanOrEqual(_result.Last().Severity);
}
