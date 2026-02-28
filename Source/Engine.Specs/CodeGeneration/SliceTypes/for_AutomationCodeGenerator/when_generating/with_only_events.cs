// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.SliceTypes.for_AutomationCodeGenerator.when_generating;

/// <summary>
/// An Automation slice that has only events and no commands or read models.
/// Only the event type renderer should be called.
/// </summary>
public class with_only_events : given.a_slice_type_code_generator
{
    AutomationCodeGenerator _generator;
    VerticalSlice _slice;
    GeneratedFile _eventFile;
    IEnumerable<GeneratedFile> _result;

    void Establish()
    {
        _generator = new AutomationCodeGenerator();
        var domainEvent = new EventType("PaymentReceived", "A payment was received", []);
        _slice = new VerticalSlice("Payments", VerticalSliceType.Automation, null, null, [], [], [domainEvent]);
        _eventFile = new GeneratedFile("Test/PaymentReceived.cs", "// event");
        _eventTypeRenderer.Render(Arg.Any<EventTypeDescriptor>(), Arg.Any<CodeGenerationContext>()).Returns([_eventFile]);
    }

    void Because() => _result = _generator.Generate(_slice, _context, _renderSet);

    [Fact] void should_call_event_type_renderer() => _eventTypeRenderer.Received(1).Render(Arg.Any<EventTypeDescriptor>(), _context);
    [Fact] void should_not_call_command_renderer() => _commandRenderer.DidNotReceiveWithAnyArgs().Render(default!, default!);
    [Fact] void should_not_call_read_model_renderer() => _readModelRenderer.DidNotReceiveWithAnyArgs().Render(default!, default!);
    [Fact] void should_return_event_file() => _result.ShouldContainOnly(_eventFile);
}
