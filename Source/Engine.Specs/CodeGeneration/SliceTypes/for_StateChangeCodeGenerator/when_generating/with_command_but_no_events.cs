// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.SliceTypes.for_StateChangeCodeGenerator.when_generating;

/// <summary>
/// A StateChange slice that has a command but an empty event list should produce
/// a command descriptor with no ProducedEvents. The command renderer then emits
/// the synchronous <c>void Handle()</c> form (no return type — nothing to append).
/// </summary>
public class with_command_but_no_events : given.a_slice_type_code_generator
{
    StateChangeCodeGenerator _generator;
    VerticalSlice _slice;
    CommandDescriptor _capturedDescriptor;

    void Establish()
    {
        _generator = new StateChangeCodeGenerator();
        var command = new Command("DeleteDraft", "Deletes a draft entry", [], "Id");
        _slice = new VerticalSlice("DeleteDraft", VerticalSliceType.StateChange, null, null, [command], [], []);

        _commandRenderer
            .Render(Arg.Do<CommandDescriptor>(d => _capturedDescriptor = d), Arg.Any<CodeGenerationContext>())
            .Returns([]);
    }

    void Because() => _generator.Generate(_slice, _context, _renderSet);

    [Fact] void should_call_command_renderer() =>
        _commandRenderer.Received(1).Render(Arg.Any<CommandDescriptor>(), _context);

    [Fact] void should_create_command_descriptor_with_no_produced_events() =>
        _capturedDescriptor.ProducedEvents.ShouldBeEmpty();
}
