// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;
using Cratis.VerticalSlices.EventModelAdvisory.Rules;

namespace Cratis.VerticalSlices.for_ExternalEventInNonTranslatorSliceRule.when_evaluating;

public class with_external_event_in_translator_slice : Specification
{
    static Module[] _modules;
    IEnumerable<EventModelRecommendation> _result;

    void Establish()
    {
        var externalEvent = new EventType("ExternalOrderPlaced", "An external order event", [], EventKind.External);
        var slice = new VerticalSlice("TranslateOrder", VerticalSliceType.Translator, null, null, [], [], [externalEvent]);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because() => _result = new ExternalEventInNonTranslatorSliceRule().Evaluate(_modules);

    [Fact] void should_return_no_recommendations() => _result.ShouldBeEmpty();
}
