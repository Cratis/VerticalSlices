// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.SliceTypes.for_AutomationCodeGenerator.when_generating;

public class with_no_artifacts : given.a_slice_type_code_generator
{
    AutomationCodeGenerator _generator;
    VerticalSlice _slice;
    IEnumerable<GeneratedFile> _result;

    void Establish()
    {
        _generator = new AutomationCodeGenerator();
        _slice = new VerticalSlice("ReviewInvoice", VerticalSliceType.Automation, null, null, [], [], []);
    }

    void Because() => _result = _generator.Generate(_slice, _context, _renderSet);

    [Fact] void should_return_no_files() => _result.ShouldBeEmpty();
    [Fact] void should_not_call_event_type_renderer() => _eventTypeRenderer.DidNotReceiveWithAnyArgs().Render(default!, default!);
    [Fact] void should_not_call_read_model_renderer() => _readModelRenderer.DidNotReceiveWithAnyArgs().Render(default!, default!);
    [Fact] void should_not_call_command_renderer() => _commandRenderer.DidNotReceiveWithAnyArgs().Render(default!, default!);
}
