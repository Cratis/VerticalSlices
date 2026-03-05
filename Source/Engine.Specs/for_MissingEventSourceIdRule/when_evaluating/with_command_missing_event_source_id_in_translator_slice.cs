// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;
using Cratis.VerticalSlices.EventModelAdvisory.Rules;

namespace Cratis.VerticalSlices.for_MissingEventSourceIdRule.when_evaluating;

public class with_command_missing_event_source_id_in_translator_slice : Specification
{
    static Module[] _modules;
    IEnumerable<EventModelRecommendation> _result;

    void Establish()
    {
        var command = new Command("TranslateOrder", "Translates an external order", [], string.Empty);
        var externalEvent = new EventType("ExternalOrderPlaced", "An external order", [], EventKind.External);
        var slice = new VerticalSlice("TranslateOrder", VerticalSliceType.Translator, null, null, [command], [], [externalEvent]);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because() => _result = new MissingEventSourceIdRule().Evaluate(_modules);

    [Fact] void should_return_no_recommendations() => _result.ShouldBeEmpty();
}
