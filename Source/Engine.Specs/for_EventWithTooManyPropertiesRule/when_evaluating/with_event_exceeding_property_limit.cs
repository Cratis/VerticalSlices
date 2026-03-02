// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;
using Cratis.VerticalSlices.EventModelAdvisory.Rules;

namespace Cratis.VerticalSlices.for_EventWithTooManyPropertiesRule.when_evaluating;

public class with_event_exceeding_property_limit : Specification
{
    static Module[] _modules;
    List<EventModelRecommendation> _result;

    void Establish()
    {
        var properties = Enumerable.Range(1, EventWithTooManyPropertiesRule.MaxRecommendedProperties + 1)
            .Select(i => new Property($"Prop{i}", "string"))
            .ToArray();
        var eventType = new EventType("OrderPlaced", "An order was placed", properties);
        var command = new Command("PlaceOrder", "Places an order", [], "OrderId");
        var slice = new VerticalSlice("PlaceOrder", VerticalSliceType.StateChange, null, null, [command], [], [eventType]);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because() => _result = new EventWithTooManyPropertiesRule().Evaluate(_modules).ToList();

    [Fact] void should_return_one_recommendation() => _result.Count.ShouldEqual(1);
    [Fact] void should_have_suggestion_severity() => _result[0].Severity.ShouldEqual(EventModelRecommendationSeverity.Suggestion);
    [Fact] void should_reference_the_event_name() => _result[0].ArtifactName.ShouldEqual("OrderPlaced");
    [Fact] void should_mention_the_count_in_message() => _result[0].Message.ShouldContain("6");
    [Fact] void should_have_module_name() => _result[0].ModuleName.ShouldEqual("Orders");
    [Fact] void should_have_feature_path_with_one_segment() => _result[0].FeaturePath.Segments.Count.ShouldEqual(1);
    [Fact] void should_have_feature_name_in_path() => _result[0].FeaturePath.Segments[0].ShouldEqual("Ordering");
}
