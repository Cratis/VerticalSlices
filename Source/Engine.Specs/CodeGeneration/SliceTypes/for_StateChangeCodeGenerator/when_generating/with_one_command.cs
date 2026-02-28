// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.SliceTypes.for_StateChangeCodeGenerator.when_generating;

public class with_one_command : given.a_slice_type_code_generator
{
    StateChangeCodeGenerator _generator;
    VerticalSlice _slice;
    GeneratedFile _expectedFile;
    IEnumerable<GeneratedFile> _result;

    void Establish()
    {
        _generator = new StateChangeCodeGenerator();
        var command = new Command("PlaceOrder", "Places an order", [new Property("OrderId", "string")], "OrderId");
        _slice = new VerticalSlice("PlaceOrder", VerticalSliceType.StateChange, null, null, [command], [], []);
        _expectedFile = new GeneratedFile("Test/PlaceOrder.cs", "// generated");
        _commandRenderer.Render(Arg.Any<CommandDescriptor>(), Arg.Any<CodeGenerationContext>()).Returns([_expectedFile]);
    }

    void Because() => _result = _generator.Generate(_slice, _context, _renderSet);

    [Fact] void should_call_command_renderer() => _commandRenderer.Received(1).Render(Arg.Any<CommandDescriptor>(), _context);
    [Fact] void should_return_the_file_from_the_renderer() => _result.ShouldContainOnly(_expectedFile);
    [Fact] void should_not_call_event_type_renderer() => _eventTypeRenderer.DidNotReceiveWithAnyArgs().Render(default!, default!);
}
