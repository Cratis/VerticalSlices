// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;
using Cratis.VerticalSlices.EventModelAdvisory.Rules;

namespace Cratis.VerticalSlices.for_MissingSourcePropertyInMappingRule.when_evaluating;

public class with_all_source_properties_present : Specification
{
    static Module[] _modules;
    IEnumerable<EventModelRecommendation> _result;

    void Establish()
    {
        var readModelProperties = new[]
        {
            new ReadModelProperty("Name", "string", [new EventPropertyMapping("OrderPlaced", EventPropertyMappingKind.Set, "Name")]),
            new ReadModelProperty("Total", "decimal", [new EventPropertyMapping("OrderPlaced", EventPropertyMappingKind.Add, "Amount")]),
            new ReadModelProperty("Count", "int", [new EventPropertyMapping("OrderPlaced", EventPropertyMappingKind.Count)])
        };
        var readModel = new ReadModel("OrderView", "Order read model", readModelProperties);
        var slice = new VerticalSlice("ViewOrders", VerticalSliceType.StateView, null, null, [], [readModel], []);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because() => _result = new MissingSourcePropertyInMappingRule().Evaluate(_modules);

    [Fact] void should_return_no_recommendations() => _result.ShouldBeEmpty();
}
