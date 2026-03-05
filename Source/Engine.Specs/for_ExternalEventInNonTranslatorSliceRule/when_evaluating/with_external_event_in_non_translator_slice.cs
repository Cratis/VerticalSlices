// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;
using Cratis.VerticalSlices.EventModelAdvisory.Rules;

namespace Cratis.VerticalSlices.for_ExternalEventInNonTranslatorSliceRule.when_evaluating;

public class with_external_event_in_non_translator_slice : Specification
{
    static Module[] _modules;
    List<EventModelRecommendation> _result;

    void Establish()
    {
        var command = new Command("HandleExternal", "Handles an external event", [], "OrderId");
        var externalEvent = new EventType("ExternalOrderPlaced", "An external order event", [], EventKind.External);
        var slice = new VerticalSlice("HandleExternal", VerticalSliceType.StateChange, null, null, [command], [], [externalEvent]);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because() => _result = new ExternalEventInNonTranslatorSliceRule().Evaluate(_modules).ToList();

    [Fact] void should_return_one_recommendation() => _result.Count.ShouldEqual(1);
    [Fact] void should_have_error_severity() => _result[0].Severity.ShouldEqual(EventModelRecommendationSeverity.Error);
    [Fact] void should_reference_the_external_event_as_artifact() => _result[0].ArtifactName.ShouldEqual("ExternalOrderPlaced");
    [Fact] void should_reference_the_slice_name() => _result[0].SliceName.ShouldEqual("HandleExternal");
    [Fact] void should_have_module_name() => _result[0].ModuleName.ShouldEqual("Orders");
    [Fact] void should_have_feature_path_with_one_segment() => _result[0].FeaturePath.Segments.Count.ShouldEqual(1);
    [Fact] void should_have_feature_name_in_path() => _result[0].FeaturePath.Segments[0].ShouldEqual("Ordering");
    [Fact] void should_reference_the_event_name_in_the_message() => _result[0].Message.ShouldContain("ExternalOrderPlaced");
}
