// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;
using Cratis.VerticalSlices.EventModelAdvisory.Rules;

namespace Cratis.VerticalSlices.for_CommandNamingConventionRule.when_evaluating;

public class with_progressive_command_name : Specification
{
    static Module[] _modules;
    List<EventModelRecommendation> _result;

    void Establish()
    {
        var command = new Command("PlacingOrder", "An order is being placed", [], "OrderId");
        var eventType = new EventType("OrderPlaced", "An order was placed", [new Property("OrderId", "Guid")]);
        var slice = new VerticalSlice("PlaceOrder", VerticalSliceType.StateChange, null, null, [command], [], [eventType]);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because() => _result = new CommandNamingConventionRule().Evaluate(_modules).ToList();

    [Fact] void should_return_one_recommendation() => _result.Count.ShouldEqual(1);
    [Fact] void should_have_suggestion_severity() => _result[0].Severity.ShouldEqual(EventModelRecommendationSeverity.Suggestion);
    [Fact] void should_reference_the_command_name() => _result[0].ArtifactName.ShouldEqual("PlacingOrder");
}
