// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.SliceTypes.for_StateViewCodeGenerator.when_generating;

public class with_one_read_model : given.a_slice_type_code_generator
{
    StateViewCodeGenerator _generator;
    VerticalSlice _slice;
    RenderedArtifact _projectionFile;
    RenderedArtifact _queryFile;
    IEnumerable<RenderedArtifact> _result;

    void Establish()
    {
        _generator = new StateViewCodeGenerator();
        var readModel = new ReadModel("Employee", "An employee", []);
        _slice = new VerticalSlice("Employees", VerticalSliceType.StateView, null, null, [], [readModel], []);
        _projectionFile = new RenderedArtifact("Test/Employee.cs", "// projection");
        _queryFile = new RenderedArtifact("Test/AllEmployees.cs", "// query");
        _readModelRenderer.Render(Arg.Any<ReadModelDescriptor>(), Arg.Any<CodeGenerationContext>())
            .Returns([_projectionFile, _queryFile]);
    }

    void Because() => _result = _generator.Generate(_slice, _context, _renderSet);

    [Fact] void should_call_read_model_renderer() => _readModelRenderer.Received(1).Render(Arg.Any<ReadModelDescriptor>(), _context);
    [Fact] void should_return_two_files() => _result.Count().ShouldEqual(2);
    [Fact] void should_include_projection_file() => _result.ShouldContain(_projectionFile);
    [Fact] void should_include_query_file() => _result.ShouldContain(_queryFile);
    [Fact] void should_not_call_event_type_renderer() => _eventTypeRenderer.DidNotReceiveWithAnyArgs().Render(default!, default!);
    [Fact] void should_not_call_command_renderer() => _commandRenderer.DidNotReceiveWithAnyArgs().Render(default!, default!);
}
