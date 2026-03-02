// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;
using Cratis.VerticalSlices.EventModelAdvisory.Rules;

namespace Cratis.VerticalSlices.for_DuplicateArtifactNameInSliceRule.when_evaluating;

public class with_unique_artifact_names : Specification
{
    static Module[] _modules;
    IEnumerable<EventModelRecommendation> _result;

    void Establish()
    {
        var command1 = new Command("PlaceOrder", "Places an order", [], "OrderId");
        var command2 = new Command("CancelOrder", "Cancels an order", [], "OrderId");
        var event1 = new EventType("OrderPlaced", "An order was placed", []);
        var event2 = new EventType("OrderCancelled", "An order was cancelled", []);
        var readModel1 = new ReadModel("OrderView", "Order view", []);
        var readModel2 = new ReadModel("OrderSummary", "Order summary", []);
        var slice = new VerticalSlice("Orders", VerticalSliceType.StateChange, null, null,
            [command1, command2], [readModel1, readModel2], [event1, event2]);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because() => _result = new DuplicateArtifactNameInSliceRule().Evaluate(_modules);

    [Fact] void should_return_no_recommendations() => _result.ShouldBeEmpty();
}
