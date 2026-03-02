// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;

namespace Cratis.VerticalSlices.for_SliceValidator.when_validating;

public class with_external_event_in_sub_feature_slice : Specification
{
    static Module[] _modules;
    Exception _exception;

    void Establish()
    {
        var externalEvent = new EventType("ExternalEvent", "External", [], EventKind.External);
        var slice = new VerticalSlice("SubSlice", VerticalSliceType.Automation, null, null, [], [], [externalEvent]);
        var subFeature = new Feature("SubFeature", [], [], [slice]);
        var feature = new Feature("Feature", [], [subFeature], []);
        _modules = [new Module("Mod", [], [feature])];
    }

    void Because()
    {
        _exception = Catch.Exception(() => new SliceValidator(new EventModelAdvisor()).Validate(_modules));
    }

    [Fact] void should_throw_slice_validation_failed() => _exception.ShouldBeOfExactType<SliceValidationFailed>();
    [Fact] void should_report_one_error() => (_exception as SliceValidationFailed)!.Errors.Count.ShouldEqual(1);
    [Fact] void should_report_the_slice_name() => (_exception as SliceValidationFailed)!.Errors[0].SliceName.ShouldEqual("SubSlice");
}
