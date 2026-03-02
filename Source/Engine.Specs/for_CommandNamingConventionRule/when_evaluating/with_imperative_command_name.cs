// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;
using Cratis.VerticalSlices.EventModelAdvisory.Rules;

namespace Cratis.VerticalSlices.for_CommandNamingConventionRule.when_evaluating;

public class with_imperative_command_name : Specification
{
    static Module[] _modules;
    IEnumerable<EventModelRecommendation> _result;

    void Establish()
    {
        var command = new Command("PlaceOrder", "Places an order", [], "OrderId");
        var eventType = new EventType("OrderPlaced", "An order was placed", [new Property("OrderId", "Guid")]);
        var slice = new VerticalSlice("PlaceOrder", VerticalSliceType.StateChange, null, null, [command], [], [eventType]);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because() => _result = new CommandNamingConventionRule().Evaluate(_modules);

    [Fact] void should_return_no_recommendations() => _result.ShouldBeEmpty();
}
