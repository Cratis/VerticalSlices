// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;
using Cratis.VerticalSlices.EventModelAdvisory.Rules;

namespace Cratis.VerticalSlices.for_StateChangeWithNoCommandsRule.when_evaluating;

public class with_state_change_slice_with_no_commands : Specification
{
    static Module[] _modules;
    List<EventModelRecommendation> _result;

    void Establish()
    {
        var internalEvent = new EventType("OrderPlaced", "An order was placed", []);
        var slice = new VerticalSlice("PlaceOrder", VerticalSliceType.StateChange, null, null, [], [], [internalEvent]);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because() => _result = new StateChangeWithNoCommandsRule().Evaluate(_modules).ToList();

    [Fact] void should_return_one_recommendation() => _result.Count.ShouldEqual(1);
    [Fact] void should_have_error_severity() => _result[0].Severity.ShouldEqual(EventModelRecommendationSeverity.Error);
    [Fact] void should_reference_the_slice_name() => _result[0].SliceName.ShouldEqual("PlaceOrder");
    [Fact] void should_have_empty_artifact_name() => _result[0].ArtifactName.ShouldEqual(string.Empty);
    [Fact] void should_have_module_name() => _result[0].ModuleName.ShouldEqual("Orders");
    [Fact] void should_have_feature_path_with_one_segment() => _result[0].FeaturePath.Segments.Count.ShouldEqual(1);
    [Fact] void should_have_feature_name_in_path() => _result[0].FeaturePath.Segments[0].ShouldEqual("Ordering");
    [Fact] void should_mention_command_in_the_message() => _result[0].Message.ShouldContain("command");
}
