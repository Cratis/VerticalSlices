// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;
using Cratis.VerticalSlices.EventModelAdvisory.Rules;

namespace Cratis.VerticalSlices.for_ReadModelWithoutKeyMappingRule.when_evaluating;

public class with_no_key_mapping : Specification
{
    static Module[] _modules;
    List<EventModelRecommendation> _result;

    void Establish()
    {
        var eventType = new EventType("OrderPlaced", "An order was placed", [new Property("OrderId", "Guid")]);
        var readModelProperties = new[]
        {
            new ReadModelProperty("Name", "string", [new EventPropertyMapping("OrderPlaced", EventPropertyMappingKind.Set, "Name")])
        };
        var readModel = new ReadModel("OrderView", "Order read model", readModelProperties);
        var slice = new VerticalSlice("ViewOrders", VerticalSliceType.StateView, null, null, [], [readModel], [eventType]);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because() => _result = new ReadModelWithoutKeyMappingRule().Evaluate(_modules).ToList();

    [Fact] void should_return_one_recommendation() => _result.Count.ShouldEqual(1);
    [Fact] void should_have_suggestion_severity() => _result[0].Severity.ShouldEqual(EventModelRecommendationSeverity.Suggestion);
    [Fact] void should_reference_the_read_model_name() => _result[0].ArtifactName.ShouldEqual("OrderView");
    [Fact] void should_be_in_best_practice_category() => _result[0].Category.ShouldEqual(EventModelRecommendationCategory.BestPractice);
}
