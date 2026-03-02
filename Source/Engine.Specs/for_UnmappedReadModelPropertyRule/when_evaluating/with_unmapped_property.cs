// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;
using Cratis.VerticalSlices.EventModelAdvisory.Rules;

namespace Cratis.VerticalSlices.for_UnmappedReadModelPropertyRule.when_evaluating;

public class with_unmapped_property : Specification
{
    static Module[] _modules;
    List<EventModelRecommendation> _result;

    void Establish()
    {
        var eventType = new EventType("OrderPlaced", "An order was placed", [new Property("OrderId", "Guid")]);
        var readModelProperties = new[]
        {
            new ReadModelProperty("Id", "Guid", [new EventPropertyMapping("OrderPlaced", EventPropertyMappingKind.FromEventSourceId)]),
            new ReadModelProperty("UnmappedField", "string", [])
        };
        var readModel = new ReadModel("OrderView", "Order read model", readModelProperties);
        var slice = new VerticalSlice("ViewOrders", VerticalSliceType.StateView, null, null, [], [readModel], [eventType]);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because() => _result = new UnmappedReadModelPropertyRule().Evaluate(_modules).ToList();

    [Fact] void should_return_one_recommendation() => _result.Count.ShouldEqual(1);
    [Fact] void should_have_information_severity() => _result[0].Severity.ShouldEqual(EventModelRecommendationSeverity.Information);
    [Fact] void should_reference_the_read_model_name() => _result[0].ArtifactName.ShouldEqual("OrderView");
    [Fact] void should_mention_the_property_name() => _result[0].Message.ShouldContain("UnmappedField");
}
