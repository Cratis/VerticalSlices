// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;
using Cratis.VerticalSlices.EventModelAdvisory.Rules;

namespace Cratis.VerticalSlices.for_DuplicateEventTypeNameRule.when_evaluating;

public class with_same_event_produced_by_two_state_change_slices : Specification
{
    static Module[] _modules;
    List<EventModelRecommendation> _result;

    void Establish()
    {
        var duplicateEvent = new EventType("OrderPlaced", "An order was placed", [new Property("OrderId", "Guid")]);
        var command = new Command("PlaceOrder", "Places an order", [], "OrderId");
        var slice1 = new VerticalSlice("PlaceOrder", VerticalSliceType.StateChange, null, null, [command], [], [duplicateEvent]);
        var slice2 = new VerticalSlice("ConfirmOrder", VerticalSliceType.StateChange, null, null, [command], [], [duplicateEvent]);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice1, slice2])])];
    }

    void Because() => _result = new DuplicateEventTypeNameRule().Evaluate(_modules).ToList();

    [Fact] void should_return_one_recommendation() => _result.Count.ShouldEqual(1);
    [Fact] void should_have_warning_severity() => _result[0].Severity.ShouldEqual(EventModelRecommendationSeverity.Warning);
    [Fact] void should_reference_the_event_name() => _result[0].ArtifactName.ShouldEqual("OrderPlaced");
    [Fact] void should_mention_both_slices_in_message() => _result[0].Message.ShouldContain("PlaceOrder");
}
