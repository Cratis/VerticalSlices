// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.for_SliceValidator.when_validating;

public class with_valid_state_change_slice : Specification
{
    static Module[] _modules;
    Exception _exception;

    void Establish()
    {
        var internalEvent = new EventType("OrderPlaced", "An order was placed", [new Property("OrderId", "Guid")]);
        var slice = new VerticalSlice("PlaceOrder", VerticalSliceType.StateChange, null, null, [], [], [internalEvent]);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because()
    {
        _exception = Catch.Exception(() => SliceValidator.Validate(_modules));
    }

    [Fact] void should_not_throw() => _exception.ShouldBeNull();
}
