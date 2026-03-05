// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;
using Cratis.VerticalSlices.EventModelAdvisory.Rules;

namespace Cratis.VerticalSlices.for_EventNamingConventionRule.when_evaluating;

public class with_irregular_past_tense_event_name : Specification
{
    static Module[] _modules;
    IEnumerable<EventModelRecommendation> _result;

    void Establish()
    {
        // "PaymentSent" — last word "Sent" is an irregular past tense in the known list
        var eventType = new EventType("PaymentSent", "A payment was sent", [new Property("Amount", "decimal")]);
        var command = new Command("SendPayment", "Sends a payment", [], "PaymentId");
        var slice = new VerticalSlice("SendPayment", VerticalSliceType.StateChange, null, null, [command], [], [eventType]);
        _modules = [new Module("Payments", [], [new Feature("Payments", [], [], [slice])])];
    }

    void Because() => _result = new EventNamingConventionRule().Evaluate(_modules);

    [Fact] void should_return_no_recommendations() => _result.ShouldBeEmpty();
}
