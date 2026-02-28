// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.SliceTypes.for_AutomationCodeGenerator.when_generating;

/// <summary>
/// An Automation slice that has only commands and no events or read models.
/// Only the command renderer should be called.
/// </summary>
public class with_only_commands : given.a_slice_type_code_generator
{
    AutomationCodeGenerator _generator;
    VerticalSlice _slice;
    RenderedArtifact _commandFile;
    IEnumerable<RenderedArtifact> _result;

    void Establish()
    {
        _generator = new AutomationCodeGenerator();
        var command = new Command("SendReminder", "Sends a reminder to the user", [], "Id");
        _slice = new VerticalSlice("Reminders", VerticalSliceType.Automation, null, null, [command], [], []);
        _commandFile = new RenderedArtifact("Test/SendReminder.cs", "// cmd");
        _commandRenderer.Render(Arg.Any<CommandDescriptor>(), Arg.Any<CodeGenerationContext>()).Returns([_commandFile]);
    }

    void Because() => _result = _generator.Generate(_slice, _context, _renderSet);

    [Fact] void should_call_command_renderer() => _commandRenderer.Received(1).Render(Arg.Any<CommandDescriptor>(), _context);
    [Fact] void should_not_call_event_type_renderer() => _eventTypeRenderer.DidNotReceiveWithAnyArgs().Render(default!, default!);
    [Fact] void should_not_call_read_model_renderer() => _readModelRenderer.DidNotReceiveWithAnyArgs().Render(default!, default!);
    [Fact] void should_return_command_file() => _result.ShouldContainOnly(_commandFile);
}
