// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.for_SliceValidator.when_validating;

public class with_external_events_in_multiple_non_translator_slices : Specification
{
    static Module[] _modules;
    SliceValidationFailed _exception;

    void Establish()
    {
        var command = new Command("HandleExternal", "Handles external event", [], "Id");
        var externalEvent = new EventType("ExternalEvent", "External", [], EventKind.External);
        var slice1 = new VerticalSlice("SliceA", VerticalSliceType.StateChange, null, null, [command], [], [externalEvent]);
        var slice2 = new VerticalSlice("SliceB", VerticalSliceType.StateView, null, null, [], [], [externalEvent]);
        _modules = [new Module("Mod", [], [new Feature("Feat", [], [], [slice1, slice2])])];
    }

    void Because()
    {
        _exception = (Catch.Exception(() => SliceValidator.Validate(_modules)) as SliceValidationFailed)!;
    }

    [Fact] void should_throw_slice_validation_failed() => _exception.ShouldNotBeNull();
    [Fact] void should_report_two_errors() => _exception.Errors.Count.ShouldEqual(2);
    [Fact] void should_report_first_slice_name() => _exception.Errors[0].SliceName.ShouldEqual("SliceA");
    [Fact] void should_report_second_slice_name() => _exception.Errors[1].SliceName.ShouldEqual("SliceB");
}
