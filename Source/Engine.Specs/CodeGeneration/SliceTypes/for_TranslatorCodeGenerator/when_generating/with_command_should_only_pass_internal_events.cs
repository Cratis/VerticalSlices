// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.SliceTypes.for_TranslatorCodeGenerator.when_generating;

/// <summary>
/// A Translator slice with a command should pass only internal events to the command
/// descriptor, ensuring external events are not included in produced event resolution.
/// </summary>
public class with_command_should_only_pass_internal_events : given.a_slice_type_code_generator
{
    TranslatorCodeGenerator _generator;
    VerticalSlice _slice;
    CommandDescriptor _capturedDescriptor;
    IEnumerable<RenderedArtifact> _result;

    void Establish()
    {
        _generator = new TranslatorCodeGenerator();
        var externalEvent = new EventType(
            "ExternalOrderPlaced",
            "External source event",
            [new Property("ExternalRef", "string")],
            EventKind.External);
        var internalEvent = new EventType(
            "OrderImported",
            "Translated internal event",
            [new Property("OrderId", "string"), new Property("ExternalRef", "string")]);
        var command = new Command(
            "ImportOrder",
            "Imports an external order",
            [new Property("ExternalRef", "string")],
            "OrderId");
        _slice = new VerticalSlice("ImportOrder", VerticalSliceType.Translator, null, null, [command], [], [externalEvent, internalEvent]);
        _commandRenderer.Render(Arg.Do<CommandDescriptor>(d => _capturedDescriptor = d), Arg.Any<CodeGenerationContext>()).Returns([]);
        _eventTypeRenderer.Render(Arg.Any<EventTypeDescriptor>(), Arg.Any<CodeGenerationContext>()).Returns([]);
    }

    void Because() => _result = _generator.Generate(_slice, _context, _renderSet);

    [Fact] void should_pass_only_internal_event_as_produced_event() => _capturedDescriptor.ProducedEvents.Count().ShouldEqual(1);
    [Fact] void should_have_internal_event_name() => _capturedDescriptor.ProducedEvents.First().EventType.Name.ShouldEqual("OrderImported");
}
