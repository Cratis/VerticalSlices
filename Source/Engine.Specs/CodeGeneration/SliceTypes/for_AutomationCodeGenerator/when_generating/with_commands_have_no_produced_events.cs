// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.SliceTypes.for_AutomationCodeGenerator.when_generating;

/// <summary>
/// Unlike StateChange slices, Automation commands are NOT associated with the events in the
/// slice — they dispatch fire-and-forget work. The CommandDescriptor must therefore be
/// created with an empty ProducedEvents list.
/// </summary>
public class with_commands_have_no_produced_events : given.a_slice_type_code_generator
{
    AutomationCodeGenerator _generator;
    VerticalSlice _slice;
    CommandDescriptor _capturedDescriptor;

    void Establish()
    {
        _generator = new AutomationCodeGenerator();
        var domainEvent = new EventType("TaskOverdue", "A task became overdue", []);
        var command = new Command("EscalateTask", "Escalates the overdue task", [], "Id");
        _slice = new VerticalSlice("Escalation", VerticalSliceType.Automation, null, null, [command], [], [domainEvent]);

        _commandRenderer
            .Render(Arg.Do<CommandDescriptor>(d => _capturedDescriptor = d), Arg.Any<CodeGenerationContext>())
            .Returns([]);
    }

    void Because() => _generator.Generate(_slice, _context, _renderSet);

    [Fact] void should_create_command_descriptor_with_no_produced_events() =>
        _capturedDescriptor.ProducedEvents.ShouldBeEmpty();
}
