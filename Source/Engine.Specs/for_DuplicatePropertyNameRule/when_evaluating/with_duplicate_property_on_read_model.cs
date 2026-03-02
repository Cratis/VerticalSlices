// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;
using Cratis.VerticalSlices.EventModelAdvisory.Rules;

namespace Cratis.VerticalSlices.for_DuplicatePropertyNameRule.when_evaluating;

public class with_duplicate_property_on_read_model : Specification
{
    static Module[] _modules;
    List<EventModelRecommendation> _result;

    void Establish()
    {
        var readModel = new ReadModel("OrderView", "Order read model",
        [
            new ReadModelProperty("Id", "Guid", [new EventPropertyMapping("OrderPlaced", EventPropertyMappingKind.FromEventSourceId)]),
            new ReadModelProperty("Id", "string", [])
        ]);
        var slice = new VerticalSlice("ViewOrders", VerticalSliceType.StateView, null, null, [], [readModel], []);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because() => _result = new DuplicatePropertyNameRule().Evaluate(_modules).ToList();

    [Fact] void should_return_one_recommendation() => _result.Count.ShouldEqual(1);
    [Fact] void should_have_error_severity() => _result[0].Severity.ShouldEqual(EventModelRecommendationSeverity.Error);
    [Fact] void should_reference_the_read_model_name() => _result[0].ArtifactName.ShouldEqual("OrderView");
    [Fact] void should_mention_the_duplicate_name() => _result[0].Message.ShouldContain("Id");
}
