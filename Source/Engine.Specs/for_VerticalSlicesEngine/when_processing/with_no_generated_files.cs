// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;
using Cratis.VerticalSlices.CodeGeneration.Renderers;

namespace Cratis.VerticalSlices.for_VerticalSlicesEngine.when_processing;

public class with_no_generated_files : given.all_dependencies
{
    VerticalSlicesEngine _engine;
    VerticalSlicesResult _result;
    IEnumerable<Module> _modules;

    void Establish()
    {
        _engine = new VerticalSlicesEngine(_codeGenerator, _advisor, _logger, _loggerFactory);

        var slice = new VerticalSlice("EmptySlice", VerticalSliceType.StateView, null, null, [], [], []);
        _modules = [new Module("Mod", [], [new Feature("Feat", [], [], [slice])])];

        _codeGenerator
            .Generate(Arg.Any<VerticalSlice>(), Arg.Any<CodeGenerationContext>(), Arg.Any<ArtifactRenderSet>())
            .Returns([]);
    }

    async Task Because() => _result = await _engine.Process(_modules);

    [Fact] void should_have_no_artifacts() => _result.Artifacts.ShouldBeEmpty();
}
