// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;
using Cratis.VerticalSlices.EventModelAdvisory.Rules;

namespace Cratis.VerticalSlices.for_DuplicateArtifactNameInSliceRule.when_evaluating;

public class with_duplicate_event_type_names : Specification
{
    static Module[] _modules;
    List<EventModelRecommendation> _result;

    void Establish()
    {
        var event1 = new EventType("OrderPlaced", "First definition", []);
        var event2 = new EventType("OrderPlaced", "Second definition", []);
        var command = new Command("PlaceOrder", "Places an order", [], "OrderId");
        var slice = new VerticalSlice("PlaceOrder", VerticalSliceType.StateChange, null, null, [command], [], [event1, event2]);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because() => _result = new DuplicateArtifactNameInSliceRule().Evaluate(_modules).ToList();

    [Fact] void should_return_one_recommendation() => _result.Count.ShouldEqual(1);
    [Fact] void should_have_error_severity() => _result[0].Severity.ShouldEqual(EventModelRecommendationSeverity.Error);
    [Fact] void should_reference_the_duplicate_event_name() => _result[0].ArtifactName.ShouldEqual("OrderPlaced");
}
