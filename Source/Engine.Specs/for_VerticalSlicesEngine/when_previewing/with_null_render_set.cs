// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;
using Cratis.VerticalSlices.CodeGeneration.Renderers;

namespace Cratis.VerticalSlices.for_VerticalSlicesEngine.when_previewing;

public class with_null_render_set : given.a_module_with_a_slice_producing_files
{
    VerticalSlicesEngine _engine;
    IEnumerable<RenderedArtifact> _result;
    ArtifactRenderSet _capturedRenderSet;

    void Establish()
    {
        _engine = new VerticalSlicesEngine(_codeGenerator, _logger);
        _codeGenerator
            .Generate(Arg.Any<VerticalSlice>(), Arg.Any<CodeGenerationContext>(), Arg.Do<ArtifactRenderSet>(rs => _capturedRenderSet = rs))
            .Returns([_generatedFile]);
    }

    void Because() => _result = _engine.Preview(_modules, renderSet: null);

    [Fact] void should_use_model_bound_render_set() => _capturedRenderSet.ShouldEqual(ArtifactRenderSet.ModelBound);
}
