// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;
using Cratis.VerticalSlices.EventModelAdvisory.Rules;

namespace Cratis.VerticalSlices.for_MissingEventSourceIdRule.when_evaluating;

public class with_command_missing_event_source_id : Specification
{
    static Module[] _modules;
    List<EventModelRecommendation> _result;

    void Establish()
    {
        var command = new Command("PlaceOrder", "Places an order", [], string.Empty);
        var internalEvent = new EventType("OrderPlaced", "An order was placed", []);
        var slice = new VerticalSlice("PlaceOrder", VerticalSliceType.StateChange, null, null, [command], [], [internalEvent]);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because() => _result = new MissingEventSourceIdRule().Evaluate(_modules).ToList();

    [Fact] void should_return_one_recommendation() => _result.Count.ShouldEqual(1);
    [Fact] void should_have_error_severity() => _result[0].Severity.ShouldEqual(EventModelRecommendationSeverity.Error);
    [Fact] void should_reference_the_command_as_artifact() => _result[0].ArtifactName.ShouldEqual("PlaceOrder");
    [Fact] void should_reference_the_slice_name() => _result[0].SliceName.ShouldEqual("PlaceOrder");
    [Fact] void should_have_module_name() => _result[0].ModuleName.ShouldEqual("Orders");
    [Fact] void should_have_feature_path_with_one_segment() => _result[0].FeaturePath.Segments.Count.ShouldEqual(1);
    [Fact] void should_have_feature_name_in_path() => _result[0].FeaturePath.Segments[0].ShouldEqual("Ordering");
    [Fact] void should_reference_command_name_in_message() => _result[0].Message.ShouldContain("PlaceOrder");
}
