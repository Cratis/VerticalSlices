// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;

namespace Cratis.VerticalSlices.for_SliceValidator.when_validating;

public class with_valid_state_change_slice : Specification
{
    static Module[] _modules;
    Exception _exception;

    void Establish()
    {
        var command = new Command("PlaceOrder", "Places an order", [], "OrderId");
        var internalEvent = new EventType("OrderPlaced", "An order was placed", [new Property("OrderId", "Guid")]);
        var slice = new VerticalSlice("PlaceOrder", VerticalSliceType.StateChange, null, null, [command], [], [internalEvent]);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because()
    {
        _exception = Catch.Exception(() => new SliceValidator(new EventModelAdvisor()).Validate(_modules));
    }

    [Fact] void should_not_throw() => _exception.ShouldBeNull();
}
