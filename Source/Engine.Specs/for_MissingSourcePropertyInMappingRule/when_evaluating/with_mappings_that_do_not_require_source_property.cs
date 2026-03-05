// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;
using Cratis.VerticalSlices.EventModelAdvisory.Rules;

namespace Cratis.VerticalSlices.for_MissingSourcePropertyInMappingRule.when_evaluating;

public class with_mappings_that_do_not_require_source_property : Specification
{
    static Module[] _modules;
    IEnumerable<EventModelRecommendation> _result;

    void Establish()
    {
        var readModelProperties = new[]
        {
            new ReadModelProperty("Id", "Guid", [new EventPropertyMapping("OrderPlaced", EventPropertyMappingKind.FromEventSourceId)]),
            new ReadModelProperty("Count", "int", [new EventPropertyMapping("OrderPlaced", EventPropertyMappingKind.Count)]),
            new ReadModelProperty("Total", "int", [new EventPropertyMapping("OrderPlaced", EventPropertyMappingKind.Increment)]),
            new ReadModelProperty("Remaining", "int", [new EventPropertyMapping("OrderPlaced", EventPropertyMappingKind.Decrement)])
        };
        var readModel = new ReadModel("OrderView", "Order read model", readModelProperties);
        var slice = new VerticalSlice("ViewOrders", VerticalSliceType.StateView, null, null, [], [readModel], []);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because() => _result = new MissingSourcePropertyInMappingRule().Evaluate(_modules);

    [Fact] void should_return_no_recommendations() => _result.ShouldBeEmpty();
}
