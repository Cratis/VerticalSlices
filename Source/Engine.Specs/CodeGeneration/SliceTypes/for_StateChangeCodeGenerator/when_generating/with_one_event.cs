// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.SliceTypes.for_StateChangeCodeGenerator.when_generating;

public class with_one_event : given.a_slice_type_code_generator
{
    StateChangeCodeGenerator _generator;
    VerticalSlice _slice;
    GeneratedFile _expectedFile;
    IEnumerable<GeneratedFile> _result;

    void Establish()
    {
        _generator = new StateChangeCodeGenerator();
        var orderPlaced = new EventType("OrderPlaced", "An order was placed", [new Property("OrderId", "string")]);
        _slice = new VerticalSlice("PlaceOrder", VerticalSliceType.StateChange, null, null, [], [], [orderPlaced]);
        _expectedFile = new GeneratedFile("Test/OrderPlaced.cs", "// generated");
        _eventTypeRenderer.Render(Arg.Any<EventTypeDescriptor>(), Arg.Any<CodeGenerationContext>()).Returns([_expectedFile]);
    }

    void Because() => _result = _generator.Generate(_slice, _context, _renderSet);

    [Fact] void should_call_event_type_renderer() => _eventTypeRenderer.Received(1).Render(Arg.Any<EventTypeDescriptor>(), _context);
    [Fact] void should_return_the_file_from_the_renderer() => _result.ShouldContainOnly(_expectedFile);
    [Fact] void should_not_call_command_renderer() => _commandRenderer.DidNotReceiveWithAnyArgs().Render(default!, default!);
}
