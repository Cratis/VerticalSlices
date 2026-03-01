// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.for_SliceValidator.when_validating;

/// <summary>
/// A StateChange slice must have at least one event. Without an event the command
/// has no observable side-effect in the domain, so the validator must reject such a slice.
/// </summary>
public class with_state_change_slice_having_no_events : Specification
{
    static Module[] _modules;
    SliceValidationFailed _exception;

    void Establish()
    {
        var command = new Command("PlaceOrder", "Places an order", [], "OrderId");
        var slice = new VerticalSlice("PlaceOrder", VerticalSliceType.StateChange, null, null, [command], [], []);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because() => _exception = Catch.Exception(() => SliceValidator.Validate(_modules)) as SliceValidationFailed;

    [Fact] void should_throw_slice_validation_failed() => _exception.ShouldNotBeNull();
    [Fact] void should_report_one_error() => _exception!.Errors.Count.ShouldEqual(1);
    [Fact] void should_report_the_slice_name() => _exception!.Errors[0].SliceName.ShouldEqual("PlaceOrder");
    [Fact] void should_report_the_slice_type() => _exception!.Errors[0].SliceType.ShouldEqual(VerticalSliceType.StateChange);
    [Fact] void should_mention_event_in_the_message() => _exception!.Errors[0].Message.ShouldContain("event");
}
