// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;

namespace Cratis.VerticalSlices.for_EventModelAdvisor.when_analyzing;

public class with_no_issues : Specification
{
    static Module[] _modules;
    IReadOnlyList<EventModelRecommendation> _result;

    void Establish()
    {
        var eventProperties = new[] { new Property("OrderId", "OrderId") };
        var internalEvent = new EventType("OrderPlaced", "An order was placed", eventProperties);
        var command = new Command("PlaceOrder", "Places an order", [new Property("OrderId", "OrderId")], "OrderId");
        var readModelProperties = new[]
        {
            new ReadModelProperty("Id", "OrderId", [new EventPropertyMapping("OrderPlaced", EventPropertyMappingKind.FromEventSourceId)])
        };
        var readModel = new ReadModel("OrderView", "Order read model", readModelProperties);
        var slice = new VerticalSlice("PlaceOrder", VerticalSliceType.StateChange, null, null, [command], [readModel], [internalEvent]);
        var concept = new Concept("OrderId", "Guid", "The unique identifier for an order", []);
        _modules = [new Module("Orders", [concept], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because() => _result = new EventModelAdvisor().Analyze(_modules);

    [Fact] void should_return_no_recommendations() => _result.ShouldBeEmpty();
}
