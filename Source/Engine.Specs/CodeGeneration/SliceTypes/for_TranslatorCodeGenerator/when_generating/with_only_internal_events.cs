// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.SliceTypes.for_TranslatorCodeGenerator.when_generating;

/// <summary>
/// A Translator slice with only internal events and no external ones.
/// All declared events should be generated since every event is internal.
/// </summary>
public class with_only_internal_events : given.a_slice_type_code_generator
{
    TranslatorCodeGenerator _generator;
    VerticalSlice _slice;
    GeneratedFile _eventFile;
    IEnumerable<GeneratedFile> _result;

    void Establish()
    {
        _generator = new TranslatorCodeGenerator();
        var internalEvent = new EventType("OrderNormalised", "Order normalised to internal format", [], EventKind.Internal);
        _slice = new VerticalSlice("Normalisation", VerticalSliceType.Translator, null, null, [], [], [internalEvent]);
        _eventFile = new GeneratedFile("Test/OrderNormalised.cs", "// event");
        _eventTypeRenderer.Render(Arg.Any<EventTypeDescriptor>(), Arg.Any<CodeGenerationContext>()).Returns([_eventFile]);
    }

    void Because() => _result = _generator.Generate(_slice, _context, _renderSet);

    [Fact] void should_call_event_type_renderer() => _eventTypeRenderer.Received(1).Render(Arg.Any<EventTypeDescriptor>(), _context);
    [Fact] void should_return_the_event_file() => _result.ShouldContainOnly(_eventFile);
    [Fact] void should_not_call_read_model_renderer() => _readModelRenderer.DidNotReceiveWithAnyArgs().Render(default!, default!);
}
