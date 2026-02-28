// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;
using Cratis.VerticalSlices.CodeGeneration.Renderers;

namespace Cratis.VerticalSlices.for_VerticalSliceCodeGenerator.when_generating;

public class with_supported_slice_type : given.all_dependencies
{
    VerticalSlice _slice;
    RenderedArtifact _expectedFile;
    IEnumerable<RenderedArtifact> _result;

    void Establish()
    {
        _slice = new VerticalSlice("MySlice", VerticalSliceType.StateChange, null, null, [], [], []);
        _expectedFile = new RenderedArtifact("MySlice/Command.cs", "// code");
        _stateChangeGenerator
            .Generate(Arg.Any<VerticalSlice>(), Arg.Any<CodeGenerationContext>(), Arg.Any<ArtifactRenderSet>())
            .Returns([_expectedFile]);
    }

    void Because() => _result = _generator.Generate(_slice, _context, _renderSet);

    [Fact] void should_return_files_from_the_slice_type_generator() => _result.ShouldContainOnly(_expectedFile);
    [Fact] void should_delegate_to_the_correct_generator() => _stateChangeGenerator.Received(1).Generate(_slice, _context, _renderSet);
}
