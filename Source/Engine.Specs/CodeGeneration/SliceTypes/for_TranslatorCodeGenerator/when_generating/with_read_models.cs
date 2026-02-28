// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.SliceTypes.for_TranslatorCodeGenerator.when_generating;

public class with_read_models : given.a_slice_type_code_generator
{
    TranslatorCodeGenerator _generator;
    VerticalSlice _slice;
    GeneratedFile _readModelFile;
    IEnumerable<GeneratedFile> _result;

    void Establish()
    {
        _generator = new TranslatorCodeGenerator();
        var externalEvent = new EventType("ExternalOrderPlaced", "External source event", [], EventKind.External);
        var taskList = new ReadModel("ImportQueue", "Pending imports", []);
        _slice = new VerticalSlice("ImportOrder", VerticalSliceType.Translator, null, null, [], [taskList], [externalEvent]);
        _readModelFile = new GeneratedFile("Test/ImportQueue.cs", "// queue");
        _readModelRenderer.Render(Arg.Any<ReadModelDescriptor>(), Arg.Any<CodeGenerationContext>()).Returns([_readModelFile]);
    }

    void Because() => _result = _generator.Generate(_slice, _context, _renderSet);

    [Fact] void should_call_read_model_renderer() => _readModelRenderer.Received(1).Render(Arg.Any<ReadModelDescriptor>(), _context);
    [Fact] void should_return_the_read_model_file() => _result.ShouldContainOnly(_readModelFile);
    [Fact] void should_not_call_event_type_renderer_for_external_event() => _eventTypeRenderer.DidNotReceiveWithAnyArgs().Render(default!, default!);
}
