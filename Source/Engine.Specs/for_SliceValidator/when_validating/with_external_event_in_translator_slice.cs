// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.for_SliceValidator.when_validating;

public class with_external_event_in_translator_slice : Specification
{
    static Module[] _modules;
    Exception _exception;

    void Establish()
    {
        var externalEvent = new EventType("ExternalOrderPlaced", "An external order event", [], EventKind.External);
        var slice = new VerticalSlice("TranslateOrder", VerticalSliceType.Translator, null, null, [], [], [externalEvent]);
        _modules = [new Module("Orders", [], [new Feature("Ordering", [], [], [slice])])];
    }

    void Because()
    {
        _exception = Catch.Exception(() => SliceValidator.Validate(_modules));
    }

    [Fact] void should_not_throw() => _exception.ShouldBeNull();
}
