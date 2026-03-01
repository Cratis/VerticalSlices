// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.for_SliceValidator.when_validating;

/// <summary>
/// A single non-Translator slice containing two external events should produce
/// two validation errors — one per external event in that slice.
/// </summary>
public class with_multiple_external_events_in_single_slice : Specification
{
    static Module[] _modules;
    SliceValidationFailed _exception;

    void Establish()
    {
        var command = new Command("ProcessOrder", "Processes the order", [], "OrderId");
        var externalEvent1 = new EventType("ExternalOrderCreated", "External", [], EventKind.External);
        var externalEvent2 = new EventType("ExternalOrderUpdated", "External", [], EventKind.External);
        var slice = new VerticalSlice("ProcessOrder", VerticalSliceType.StateChange, null, null, [command], [], [externalEvent1, externalEvent2]);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because()
    {
        _exception = (Catch.Exception(() => SliceValidator.Validate(_modules)) as SliceValidationFailed)!;
    }

    [Fact] void should_throw_slice_validation_failed() => _exception.ShouldNotBeNull();
    [Fact] void should_report_two_errors() => _exception.Errors.Count.ShouldEqual(2);
    [Fact] void should_report_first_external_event_name_in_message() =>
        _exception.Errors[0].Message.ShouldContain("ExternalOrderCreated");
    [Fact] void should_report_second_external_event_name_in_message() =>
        _exception.Errors[1].Message.ShouldContain("ExternalOrderUpdated");
    [Fact] void both_errors_should_reference_the_same_slice() =>
        _exception.Errors.All(e => e.SliceName == "ProcessOrder").ShouldBeTrue();
}
