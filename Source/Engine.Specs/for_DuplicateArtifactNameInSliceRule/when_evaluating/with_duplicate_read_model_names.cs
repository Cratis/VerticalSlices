// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;
using Cratis.VerticalSlices.EventModelAdvisory.Rules;

namespace Cratis.VerticalSlices.for_DuplicateArtifactNameInSliceRule.when_evaluating;

public class with_duplicate_read_model_names : Specification
{
    static Module[] _modules;
    List<EventModelRecommendation> _result;

    void Establish()
    {
        var readModel1 = new ReadModel("OrderView", "First read model", []);
        var readModel2 = new ReadModel("OrderView", "Second read model", []);
        var slice = new VerticalSlice("ViewOrders", VerticalSliceType.StateView, null, null, [], [readModel1, readModel2], []);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because() => _result = new DuplicateArtifactNameInSliceRule().Evaluate(_modules).ToList();

    [Fact] void should_return_one_recommendation() => _result.Count.ShouldEqual(1);
    [Fact] void should_have_error_severity() => _result[0].Severity.ShouldEqual(EventModelRecommendationSeverity.Error);
    [Fact] void should_reference_the_duplicate_name() => _result[0].ArtifactName.ShouldEqual("OrderView");
}
