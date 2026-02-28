// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.SliceTypes.for_TranslatorCodeGenerator.when_generating;

public class with_internal_and_external_events : given.a_slice_type_code_generator
{
    TranslatorCodeGenerator _generator;
    VerticalSlice _slice;
    GeneratedFile _internalEventFile;
    IEnumerable<GeneratedFile> _result;

    void Establish()
    {
        _generator = new TranslatorCodeGenerator();
        var externalEvent = new EventType("ExternalOrderPlaced", "External source event", [], EventKind.External);
        var internalEvent = new EventType("OrderImported", "Translated internal event", []);
        _slice = new VerticalSlice("ImportOrder", VerticalSliceType.Translator, null, null, [], [], [externalEvent, internalEvent]);
        _internalEventFile = new GeneratedFile("Test/OrderImported.cs", "// translated");
        _eventTypeRenderer.Render(Arg.Any<EventTypeDescriptor>(), Arg.Any<CodeGenerationContext>()).Returns([_internalEventFile]);
    }

    void Because() => _result = _generator.Generate(_slice, _context, _renderSet);

    [Fact] void should_call_event_type_renderer_once_for_the_internal_event() => _eventTypeRenderer.Received(1).Render(Arg.Any<EventTypeDescriptor>(), _context);
    [Fact] void should_return_only_the_internal_event_file() => _result.ShouldContainOnly(_internalEventFile);
}
