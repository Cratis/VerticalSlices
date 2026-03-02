// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;
using Cratis.VerticalSlices.EventModelAdvisory.Rules;

namespace Cratis.VerticalSlices.for_DuplicatePropertyNameRule.when_evaluating;

public class with_duplicate_property_on_event_type : Specification
{
    static Module[] _modules;
    List<EventModelRecommendation> _result;

    void Establish()
    {
        var eventType = new EventType("OrderPlaced", "An order was placed",
        [
            new Property("OrderId", "Guid"),
            new Property("OrderId", "string")
        ]);
        var command = new Command("PlaceOrder", "Places an order", [], "OrderId");
        var slice = new VerticalSlice("PlaceOrder", VerticalSliceType.StateChange, null, null, [command], [], [eventType]);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because() => _result = new DuplicatePropertyNameRule().Evaluate(_modules).ToList();

    [Fact] void should_return_one_recommendation() => _result.Count.ShouldEqual(1);
    [Fact] void should_have_error_severity() => _result[0].Severity.ShouldEqual(EventModelRecommendationSeverity.Error);
    [Fact] void should_reference_the_event_name() => _result[0].ArtifactName.ShouldEqual("OrderPlaced");
    [Fact] void should_mention_the_duplicate_name() => _result[0].Message.ShouldContain("OrderId");
}
