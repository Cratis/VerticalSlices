// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;
using Cratis.VerticalSlices.EventModelAdvisory.Rules;

namespace Cratis.VerticalSlices.for_PrimitivePropertyTypeRule.when_evaluating;

public class with_primitive_types_on_events : Specification
{
    static Module[] _modules;
    List<EventModelRecommendation> _result;

    void Establish()
    {
        var eventType = new EventType("OrderPlaced", "An order was placed",
        [
            new Property("OrderId", "Guid"),
            new Property("CustomerName", "string")
        ]);
        var command = new Command("PlaceOrder", "Places an order", [], "OrderId");
        var slice = new VerticalSlice("PlaceOrder", VerticalSliceType.StateChange, null, null, [command], [], [eventType]);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because() => _result = new PrimitivePropertyTypeRule().Evaluate(_modules).ToList();

    [Fact] void should_return_two_recommendations() => _result.Count.ShouldEqual(2);
    [Fact] void should_have_suggestion_severity() => _result[0].Severity.ShouldEqual(EventModelRecommendationSeverity.Suggestion);
    [Fact] void should_be_in_best_practice_category() => _result[0].Category.ShouldEqual(EventModelRecommendationCategory.BestPractice);
    [Fact] void should_mention_the_property_name() => _result[0].Message.ShouldContain("OrderId");
}
