// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;

namespace Cratis.VerticalSlices.for_SliceValidator.when_validating;

public class with_command_missing_event_source_id : Specification
{
    static Module[] _modules;
    Exception _exception;

    void Establish()
    {
        var command = new Command("PlaceOrder", "Places an order", [], string.Empty);
        var internalEvent = new EventType("OrderPlaced", "An order was placed", []);
        var slice = new VerticalSlice("PlaceOrder", VerticalSliceType.StateChange, null, null, [command], [], [internalEvent]);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because()
    {
        _exception = Catch.Exception(() => new SliceValidator(new EventModelAdvisor()).Validate(_modules));
    }

    [Fact] void should_throw_slice_validation_failed() => _exception.ShouldBeOfExactType<SliceValidationFailed>();
    [Fact] void should_report_one_error() => (_exception as SliceValidationFailed)!.Errors.Count.ShouldEqual(1);
    [Fact] void should_report_the_slice_name() => (_exception as SliceValidationFailed)!.Errors[0].SliceName.ShouldEqual("PlaceOrder");
    [Fact] void should_report_the_slice_type() => (_exception as SliceValidationFailed)!.Errors[0].SliceType.ShouldEqual(VerticalSliceType.StateChange);
    [Fact] void should_reference_the_command_name_in_the_message() => (_exception as SliceValidationFailed)!.Errors[0].Message.ShouldContain("PlaceOrder");
}
