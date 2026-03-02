// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;
using Cratis.VerticalSlices.EventModelAdvisory.Rules;

namespace Cratis.VerticalSlices.for_EventSourceIdPropertyNotFoundRule.when_evaluating;

public class with_supplied_strategy_and_no_matching_property : Specification
{
    static Module[] _modules;
    List<EventModelRecommendation> _result;

    void Establish()
    {
        var command = new Command("PlaceOrder", "Places an order",
        [
            new Property("Amount", "decimal")
        ], "OrderId", EventSourceIdStrategy.Supplied);
        var slice = new VerticalSlice("PlaceOrder", VerticalSliceType.StateChange, null, null, [command], [], []);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because() => _result = new EventSourceIdPropertyNotFoundRule().Evaluate(_modules).ToList();

    [Fact] void should_return_one_recommendation() => _result.Count.ShouldEqual(1);
    [Fact] void should_have_error_severity() => _result[0].Severity.ShouldEqual(EventModelRecommendationSeverity.Error);
    [Fact] void should_reference_the_command_name() => _result[0].ArtifactName.ShouldEqual("PlaceOrder");
    [Fact] void should_mention_the_missing_property_name() => _result[0].Message.ShouldContain("OrderId");
}
