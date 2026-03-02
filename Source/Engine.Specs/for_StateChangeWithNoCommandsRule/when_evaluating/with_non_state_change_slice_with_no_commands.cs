// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;
using Cratis.VerticalSlices.EventModelAdvisory.Rules;

namespace Cratis.VerticalSlices.for_StateChangeWithNoCommandsRule.when_evaluating;

public class with_non_state_change_slice_with_no_commands : Specification
{
    static Module[] _modules;
    IEnumerable<EventModelRecommendation> _result;

    void Establish()
    {
        var internalEvent = new EventType("InventoryRestocked", "Stock was replenished", []);
        var slice = new VerticalSlice("RestockInventory", VerticalSliceType.Automation, null, null, [], [], [internalEvent]);
        _modules = [new Module("Warehouse", [], [new Feature("Inventory", [], [], [slice])])];
    }

    void Because() => _result = new StateChangeWithNoCommandsRule().Evaluate(_modules);

    [Fact] void should_return_no_recommendations() => _result.ShouldBeEmpty();
}
