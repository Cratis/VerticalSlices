// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.for_SliceValidator.when_validating;

public class with_external_event_in_non_translator_slice : Specification
{
    static Module[] _modules;
    Exception _exception;

    void Establish()
    {
        var externalEvent = new EventType("ExternalOrderPlaced", "An external order event", [], EventKind.External);
        var slice = new VerticalSlice("HandleExternal", VerticalSliceType.StateChange, null, null, [], [], [externalEvent]);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because()
    {
        _exception = Catch.Exception(() => SliceValidator.Validate(_modules));
    }

    [Fact] void should_throw_slice_validation_failed() => _exception.ShouldBeOfExactType<SliceValidationFailed>();
    [Fact] void should_report_one_error() => (_exception as SliceValidationFailed)!.Errors.Count.ShouldEqual(1);
    [Fact] void should_report_the_slice_name() => (_exception as SliceValidationFailed)!.Errors[0].SliceName.ShouldEqual("HandleExternal");
    [Fact] void should_report_the_slice_type() => (_exception as SliceValidationFailed)!.Errors[0].SliceType.ShouldEqual(VerticalSliceType.StateChange);
    [Fact] void should_reference_the_event_name_in_the_message() => (_exception as SliceValidationFailed)!.Errors[0].Message.ShouldContain("ExternalOrderPlaced");
}
