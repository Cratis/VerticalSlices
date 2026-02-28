// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;
using Cratis.VerticalSlices.CodeGeneration.Renderers;

namespace Cratis.VerticalSlices.for_VerticalSliceCodeGenerator.when_generating;

public class with_unsupported_slice_type : given.all_dependencies
{
    VerticalSlice _slice;
    IEnumerable<RenderedArtifact> _result;

    void Establish() => _slice = new VerticalSlice("MySlice", VerticalSliceType.Automation, null, null, [], [], []);

    void Because() => _result = _generator.Generate(_slice, _context, _renderSet);

    [Fact] void should_return_empty_collection() => _result.ShouldBeEmpty();
    [Fact] void should_not_call_state_change_generator() => _stateChangeGenerator.DidNotReceive().Generate(Arg.Any<VerticalSlice>(), Arg.Any<CodeGenerationContext>(), Arg.Any<ArtifactRenderSet>());
}
