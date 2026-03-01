// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.SliceTypes.for_TranslatorCodeGenerator.when_generating;

/// <summary>
/// A Translator slice with a command that produces internal events.
/// The command renderer should be called with only the internal events,
/// and the event type renderer should still generate the internal events.
/// </summary>
public class with_command_and_internal_event : given.a_slice_type_code_generator
{
    TranslatorCodeGenerator _generator;
    VerticalSlice _slice;
    RenderedArtifact _eventFile;
    RenderedArtifact _commandFile;
    IEnumerable<RenderedArtifact> _result;

    void Establish()
    {
        _generator = new TranslatorCodeGenerator();
        var externalEvent = new EventType("ExternalOrderPlaced", "External source event", [], EventKind.External);
        var internalEvent = new EventType("OrderImported", "Translated internal event",
            [new Property("OrderId", "string"), new Property("ExternalRef", "string")]);
        var command = new Command("ImportOrder", "Imports an external order",
            [new Property("ExternalRef", "string")], "OrderId");
        _slice = new VerticalSlice("ImportOrder", VerticalSliceType.Translator, null, null, [command], [], [externalEvent, internalEvent]);
        _eventFile = new RenderedArtifact("Test/OrderImported.cs", "// event");
        _commandFile = new RenderedArtifact("Test/ImportOrder.cs", "// command");
        _eventTypeRenderer.Render(Arg.Any<EventTypeDescriptor>(), Arg.Any<CodeGenerationContext>()).Returns([_eventFile]);
        _commandRenderer.Render(Arg.Any<CommandDescriptor>(), Arg.Any<CodeGenerationContext>()).Returns([_commandFile]);
    }

    void Because() => _result = _generator.Generate(_slice, _context, _renderSet);

    [Fact] void should_call_event_type_renderer_for_internal_event() => _eventTypeRenderer.Received(1).Render(Arg.Any<EventTypeDescriptor>(), _context);
    [Fact] void should_call_command_renderer() => _commandRenderer.Received(1).Render(Arg.Any<CommandDescriptor>(), _context);
    [Fact] void should_return_both_event_and_command_files() => _result.ShouldContain(_eventFile);
    [Fact] void should_include_command_file_in_result() => _result.ShouldContain(_commandFile);
}
