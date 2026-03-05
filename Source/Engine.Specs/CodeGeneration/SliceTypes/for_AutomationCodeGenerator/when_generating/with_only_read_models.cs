// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.SliceTypes.for_AutomationCodeGenerator.when_generating;

/// <summary>
/// An Automation slice that has only read models (no events, no commands).
/// Only the read model renderer should be called.
/// </summary>
public class with_only_read_models : given.a_slice_type_code_generator
{
    AutomationCodeGenerator _generator;
    VerticalSlice _slice;
    RenderedArtifact _readModelFile;
    IEnumerable<RenderedArtifact> _result;

    void Establish()
    {
        _generator = new AutomationCodeGenerator();
        var readModel = new ReadModel("TaskSummary", "Summary of automation tasks", []);
        _slice = new VerticalSlice("TaskAutomation", VerticalSliceType.Automation, null, null, [], [readModel], []);
        _readModelFile = new RenderedArtifact("Test/TaskSummary.cs", "// rm");
        _readModelRenderer.Render(Arg.Any<ReadModelDescriptor>(), Arg.Any<CodeGenerationContext>()).Returns([_readModelFile]);
    }

    void Because() => _result = _generator.Generate(_slice, _context, _renderSet);

    [Fact] void should_call_read_model_renderer() =>
        _readModelRenderer.Received(1).Render(Arg.Any<ReadModelDescriptor>(), _context);

    [Fact] void should_not_call_command_renderer() =>
        _commandRenderer.DidNotReceiveWithAnyArgs().Render(default!, default!);

    [Fact] void should_not_call_event_type_renderer() =>
        _eventTypeRenderer.DidNotReceiveWithAnyArgs().Render(default!, default!);

    [Fact] void should_return_read_model_file() =>
        _result.ShouldContainOnly(_readModelFile);
}
