// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;
using Cratis.VerticalSlices.CodeGeneration.Renderers;

namespace Cratis.VerticalSlices.for_VerticalSliceCodeGenerator.when_generating;

public class with_null_render_set : given.all_dependencies
{
    VerticalSlice _slice;
    ArtifactRenderSet _capturedRenderSet;

    void Establish()
    {
        _slice = new VerticalSlice("MySlice", VerticalSliceType.StateChange, null, null, [], [], []);
        _stateChangeGenerator
            .Generate(Arg.Any<VerticalSlice>(), Arg.Any<CodeGenerationContext>(), Arg.Do<ArtifactRenderSet>(rs => _capturedRenderSet = rs))
            .Returns([]);
    }

    void Because() => _generator.Generate(_slice, _context, renderSet: null);

    [Fact] void should_use_model_bound_render_set() => _capturedRenderSet.ShouldEqual(ArtifactRenderSet.ModelBound);
}
