// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;
using Cratis.VerticalSlices.EventModelAdvisory.Rules;

namespace Cratis.VerticalSlices.for_StateViewWithNoReadModelsRule.when_evaluating;

public class with_state_view_without_read_models : Specification
{
    static Module[] _modules;
    List<EventModelRecommendation> _result;

    void Establish()
    {
        var eventType = new EventType("OrderPlaced", "An order was placed", [new Property("OrderId", "Guid")]);
        var slice = new VerticalSlice("ViewOrders", VerticalSliceType.StateView, null, null, [], [], [eventType]);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because() => _result = new StateViewWithNoReadModelsRule().Evaluate(_modules).ToList();

    [Fact] void should_return_one_recommendation() => _result.Count.ShouldEqual(1);
    [Fact] void should_have_warning_severity() => _result[0].Severity.ShouldEqual(EventModelRecommendationSeverity.Warning);
    [Fact] void should_reference_the_slice_name() => _result[0].SliceName.ShouldEqual("ViewOrders");
    [Fact] void should_mention_the_slice_in_message() => _result[0].Message.ShouldContain("ViewOrders");
}
