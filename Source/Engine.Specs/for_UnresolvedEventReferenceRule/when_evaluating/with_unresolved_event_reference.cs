// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;
using Cratis.VerticalSlices.EventModelAdvisory.Rules;

namespace Cratis.VerticalSlices.for_UnresolvedEventReferenceRule.when_evaluating;

public class with_unresolved_event_reference : Specification
{
    static Module[] _modules;
    List<EventModelRecommendation> _result;

    void Establish()
    {
        var readModelProperties = new[]
        {
            new ReadModelProperty("Name", "string", [new EventPropertyMapping("NonExistentEvent", EventPropertyMappingKind.Set, "Name")])
        };
        var readModel = new ReadModel("OrderView", "Order read model", readModelProperties);
        var slice = new VerticalSlice("ViewOrders", VerticalSliceType.StateView, null, null, [], [readModel], []);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because() => _result = new UnresolvedEventReferenceRule().Evaluate(_modules).ToList();

    [Fact] void should_return_one_recommendation() => _result.Count.ShouldEqual(1);
    [Fact] void should_have_error_severity() => _result[0].Severity.ShouldEqual(EventModelRecommendationSeverity.Error);
    [Fact] void should_reference_the_read_model_name() => _result[0].ArtifactName.ShouldEqual("OrderView");
    [Fact] void should_mention_the_missing_event_in_message() => _result[0].Message.ShouldContain("NonExistentEvent");
}
