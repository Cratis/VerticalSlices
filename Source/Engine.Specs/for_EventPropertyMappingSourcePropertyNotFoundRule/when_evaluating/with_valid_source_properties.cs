// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;
using Cratis.VerticalSlices.EventModelAdvisory.Rules;

namespace Cratis.VerticalSlices.for_EventPropertyMappingSourcePropertyNotFoundRule.when_evaluating;

public class with_valid_source_properties : Specification
{
    static Module[] _modules;
    IEnumerable<EventModelRecommendation> _result;

    void Establish()
    {
        var eventType = new EventType("OrderPlaced", "An order was placed", [new Property("Amount", "decimal"), new Property("OrderId", "Guid")]);
        var readModelProp = new ReadModelProperty("Total", "decimal", [new EventPropertyMapping("OrderPlaced", EventPropertyMappingKind.Set, "Amount")]);
        var readModel = new ReadModel("OrderView", "Order view", [readModelProp]);
        var slice = new VerticalSlice("ViewOrders", VerticalSliceType.StateView, null, null, [], [readModel], [eventType]);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because() => _result = new EventPropertyMappingSourcePropertyNotFoundRule().Evaluate(_modules);

    [Fact] void should_return_no_recommendations() => _result.ShouldBeEmpty();
}
