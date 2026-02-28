// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.SliceTypes.for_StateViewCodeGenerator.when_generating;

public class with_multiple_read_models : given.a_slice_type_code_generator
{
    StateViewCodeGenerator _generator;
    VerticalSlice _slice;
    RenderedArtifact _file1;
    RenderedArtifact _file2;
    IEnumerable<RenderedArtifact> _result;

    void Establish()
    {
        _generator = new StateViewCodeGenerator();
        var readModel1 = new ReadModel("Employee", "An employee", []);
        var readModel2 = new ReadModel("Department", "A department", []);
        _slice = new VerticalSlice("HR", VerticalSliceType.StateView, null, null, [], [readModel1, readModel2], []);
        _file1 = new RenderedArtifact("Test/Employee.cs", "// rm1");
        _file2 = new RenderedArtifact("Test/Department.cs", "// rm2");
        _readModelRenderer.Render(Arg.Any<ReadModelDescriptor>(), Arg.Any<CodeGenerationContext>())
            .Returns([_file1], [_file2]);
    }

    void Because() => _result = _generator.Generate(_slice, _context, _renderSet);

    [Fact] void should_call_read_model_renderer_for_each_read_model() => _readModelRenderer.Received(2).Render(Arg.Any<ReadModelDescriptor>(), _context);
    [Fact] void should_return_all_files() => _result.Count().ShouldEqual(2);
    [Fact] void should_include_first_read_model_file() => _result.ShouldContain(_file1);
    [Fact] void should_include_second_read_model_file() => _result.ShouldContain(_file2);
}
