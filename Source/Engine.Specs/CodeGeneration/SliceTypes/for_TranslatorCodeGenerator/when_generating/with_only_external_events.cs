// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.SliceTypes.for_TranslatorCodeGenerator.when_generating;

public class with_only_external_events : given.a_slice_type_code_generator
{
    TranslatorCodeGenerator _generator;
    VerticalSlice _slice;
    IEnumerable<GeneratedFile> _result;

    void Establish()
    {
        _generator = new TranslatorCodeGenerator();
        var externalEvent = new EventType("ExternalOrderPlaced", "An external order event", [], EventKind.External);
        _slice = new VerticalSlice("ImportOrder", VerticalSliceType.Translator, null, null, [], [], [externalEvent]);
    }

    void Because() => _result = _generator.Generate(_slice, _context, _renderSet);

    [Fact] void should_return_no_files() => _result.ShouldBeEmpty();
    [Fact] void should_not_call_event_type_renderer() => _eventTypeRenderer.DidNotReceiveWithAnyArgs().Render(default!, default!);
}
