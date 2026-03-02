// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;
using Cratis.VerticalSlices.EventModelAdvisory.Rules;

namespace Cratis.VerticalSlices.for_EventPropertyMappingSourcePropertyNotFoundRule.when_evaluating;

public class with_mapping_not_requiring_source_property : Specification
{
    static Module[] _modules;
    IEnumerable<EventModelRecommendation> _result;

    void Establish()
    {
        var eventType = new EventType("OrderPlaced", "An order was placed", [new Property("OrderId", "Guid")]);
        var readModelProp = new ReadModelProperty("OrderCount", "int", [new EventPropertyMapping("OrderPlaced", EventPropertyMappingKind.Count)]);
        var readModel = new ReadModel("OrderView", "Order view", [readModelProp]);
        var slice = new VerticalSlice("ViewOrders", VerticalSliceType.StateView, null, null, [], [readModel], [eventType]);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because() => _result = new EventPropertyMappingSourcePropertyNotFoundRule().Evaluate(_modules);

    [Fact] void should_return_no_recommendations() => _result.ShouldBeEmpty();
}
