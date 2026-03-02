// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;
using Cratis.VerticalSlices.EventModelAdvisory.Rules;

namespace Cratis.VerticalSlices.for_OrphanedEventRule.when_evaluating;

public class with_event_not_referenced_by_any_read_model : Specification
{
    static Module[] _modules;
    List<EventModelRecommendation> _result;

    void Establish()
    {
        var eventType = new EventType("OrderPlaced", "An order was placed", [new Property("OrderId", "Guid")]);
        var command = new Command("PlaceOrder", "Places an order", [], "OrderId");
        var slice = new VerticalSlice("PlaceOrder", VerticalSliceType.StateChange, null, null, [command], [], [eventType]);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because() => _result = new OrphanedEventRule().Evaluate(_modules).ToList();

    [Fact] void should_return_one_recommendation() => _result.Count.ShouldEqual(1);
    [Fact] void should_have_suggestion_severity() => _result[0].Severity.ShouldEqual(EventModelRecommendationSeverity.Suggestion);
    [Fact] void should_reference_the_event_name() => _result[0].ArtifactName.ShouldEqual("OrderPlaced");
    [Fact] void should_be_in_coverage_category() => _result[0].Category.ShouldEqual(EventModelRecommendationCategory.Coverage);
}
