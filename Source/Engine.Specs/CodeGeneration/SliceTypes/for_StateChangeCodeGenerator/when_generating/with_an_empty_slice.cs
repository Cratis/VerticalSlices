// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.SliceTypes.for_StateChangeCodeGenerator.when_generating;

public class with_an_empty_slice : given.a_slice_type_code_generator
{
    StateChangeCodeGenerator _generator;
    VerticalSlice _slice;
    IEnumerable<RenderedArtifact> _result;

    void Establish()
    {
        _generator = new StateChangeCodeGenerator();
        _slice = new VerticalSlice("Empty", VerticalSliceType.StateChange, null, null, [], [], []);
    }

    void Because() => _result = _generator.Generate(_slice, _context, _renderSet);

    [Fact] void should_return_no_files() => _result.ShouldBeEmpty();
}
