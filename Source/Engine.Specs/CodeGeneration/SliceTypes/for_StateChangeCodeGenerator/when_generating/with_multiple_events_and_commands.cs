// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.SliceTypes.for_StateChangeCodeGenerator.when_generating;

public class with_multiple_events_and_commands : given.a_slice_type_code_generator
{
    StateChangeCodeGenerator _generator;
    VerticalSlice _slice;
    GeneratedFile _eventFile1;
    GeneratedFile _eventFile2;
    GeneratedFile _commandFile;
    IEnumerable<GeneratedFile> _result;

    void Establish()
    {
        _generator = new StateChangeCodeGenerator();
        var event1 = new EventType("OrderPlaced", "An order was placed", []);
        var event2 = new EventType("OrderShipped", "An order was shipped", []);
        var command = new Command("PlaceOrder", "Places an order", [], "Id");
        _slice = new VerticalSlice("PlaceOrder", VerticalSliceType.StateChange, null, null, [command], [], [event1, event2]);
        _eventFile1 = new GeneratedFile("Test/OrderPlaced.cs", "// event1");
        _eventFile2 = new GeneratedFile("Test/OrderShipped.cs", "// event2");
        _commandFile = new GeneratedFile("Test/PlaceOrder.cs", "// command");
        _eventTypeRenderer.Render(Arg.Any<EventTypeDescriptor>(), Arg.Any<CodeGenerationContext>())
            .Returns([_eventFile1], [_eventFile2]);
        _commandRenderer.Render(Arg.Any<CommandDescriptor>(), Arg.Any<CodeGenerationContext>())
            .Returns([_commandFile]);
    }

    void Because() => _result = _generator.Generate(_slice, _context, _renderSet);

    [Fact] void should_call_event_type_renderer_for_each_event() => _eventTypeRenderer.Received(2).Render(Arg.Any<EventTypeDescriptor>(), _context);
    [Fact] void should_call_command_renderer_once() => _commandRenderer.Received(1).Render(Arg.Any<CommandDescriptor>(), _context);
    [Fact] void should_return_three_files() => _result.Count().ShouldEqual(3);
    [Fact] void should_include_first_event_file() => _result.ShouldContain(_eventFile1);
    [Fact] void should_include_second_event_file() => _result.ShouldContain(_eventFile2);
    [Fact] void should_include_command_file() => _result.ShouldContain(_commandFile);
}
