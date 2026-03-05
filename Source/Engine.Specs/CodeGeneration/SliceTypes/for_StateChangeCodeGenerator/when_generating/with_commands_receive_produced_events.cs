// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.SliceTypes.for_StateChangeCodeGenerator.when_generating;

/// <summary>
/// Unlike Automation slices, StateChange commands ARE associated with the events in the slice —
/// the command is the trigger that causes those events. The CommandDescriptor must therefore
/// be created with the full slice event list as ProducedEvents.
/// </summary>
public class with_commands_receive_produced_events : given.a_slice_type_code_generator
{
    StateChangeCodeGenerator _generator;
    VerticalSlice _slice;
    CommandDescriptor _capturedDescriptor;

    void Establish()
    {
        _generator = new StateChangeCodeGenerator();
        var domainEvent = new EventType("OrderPlaced", "Order was placed", []);
        var command = new Command("PlaceOrder", "Places a customer order", [], "Id");
        _slice = new VerticalSlice("PlaceOrder", VerticalSliceType.StateChange, null, null, [command], [], [domainEvent]);

        _commandRenderer
            .Render(Arg.Do<CommandDescriptor>(d => _capturedDescriptor = d), Arg.Any<CodeGenerationContext>())
            .Returns([]);
    }

    void Because() => _generator.Generate(_slice, _context, _renderSet);

    [Fact] void should_create_command_descriptor_with_produced_events() =>
        _capturedDescriptor.ProducedEvents.Count().ShouldEqual(1);

    [Fact] void should_include_the_domain_event_as_produced_event() =>
        _capturedDescriptor.ProducedEvents.First().EventType.Name.ShouldEqual("OrderPlaced");
}
