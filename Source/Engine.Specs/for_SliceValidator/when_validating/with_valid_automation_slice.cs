// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;

namespace Cratis.VerticalSlices.for_SliceValidator.when_validating;

/// <summary>
/// An Automation slice that contains only internal events is valid — the validator's
/// early-return guard for Translator slices does not apply, but the loop that
/// checks for external events finds none, so no errors should be reported.
/// </summary>
public class with_valid_automation_slice : Specification
{
    static Module[] _modules;
    Exception _exception;

    void Establish()
    {
        var internalEvent = new EventType("InventoryRestocked", "Stock was replenished", [new Property("ItemId", "Guid")]);
        var slice = new VerticalSlice("RestockInventory", VerticalSliceType.Automation, null, null, [], [], [internalEvent]);
        _modules = [new Module("Warehouse", [], [new Feature("Inventory", [], [], [slice])])];
    }

    void Because()
    {
        _exception = Catch.Exception(() => new SliceValidator(new EventModelAdvisor()).Validate(_modules));
    }

    [Fact] void should_not_throw() => _exception.ShouldBeNull();
}
