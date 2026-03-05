// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;
using Cratis.VerticalSlices.EventModelAdvisory.Rules;

namespace Cratis.VerticalSlices.for_EventPropertyMappingSourcePropertyNotFoundRule.when_evaluating;

public class with_source_property_not_on_event_type : Specification
{
    static Module[] _modules;
    List<EventModelRecommendation> _result;

    void Establish()
    {
        var eventType = new EventType("OrderPlaced", "An order was placed", [new Property("OrderId", "Guid")]);
        var readModelProp = new ReadModelProperty("Total", "decimal", [new EventPropertyMapping("OrderPlaced", EventPropertyMappingKind.Set, "Amount")]);
        var readModel = new ReadModel("OrderView", "Order view", [readModelProp]);
        var slice = new VerticalSlice("ViewOrders", VerticalSliceType.StateView, null, null, [], [readModel], [eventType]);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because() => _result = new EventPropertyMappingSourcePropertyNotFoundRule().Evaluate(_modules).ToList();

    [Fact] void should_return_one_recommendation() => _result.Count.ShouldEqual(1);
    [Fact] void should_have_error_severity() => _result[0].Severity.ShouldEqual(EventModelRecommendationSeverity.Error);
    [Fact] void should_reference_the_read_model_name() => _result[0].ArtifactName.ShouldEqual("OrderView");
    [Fact] void should_mention_the_missing_property_in_message() => _result[0].Message.ShouldContain("Amount");
    [Fact] void should_mention_the_event_name_in_message() => _result[0].Message.ShouldContain("OrderPlaced");
}
