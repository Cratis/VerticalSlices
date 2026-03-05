// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.SliceTypes.for_StateViewCodeGenerator.when_generating;

public class with_no_read_models : given.a_slice_type_code_generator
{
    StateViewCodeGenerator _generator;
    VerticalSlice _slice;
    IEnumerable<RenderedArtifact> _result;

    void Establish()
    {
        _generator = new StateViewCodeGenerator();
        _slice = new VerticalSlice("Employees", VerticalSliceType.StateView, null, null, [], [], []);
    }

    void Because() => _result = _generator.Generate(_slice, _context, _renderSet);

    [Fact] void should_return_no_files() => _result.ShouldBeEmpty();
}
