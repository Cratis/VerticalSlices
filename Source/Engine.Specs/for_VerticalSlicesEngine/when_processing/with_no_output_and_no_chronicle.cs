// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;
using Cratis.VerticalSlices.CodeGeneration.Renderers;

namespace Cratis.VerticalSlices.for_VerticalSlicesEngine.when_processing;

public class with_no_output_and_no_chronicle : given.a_module_with_a_slice_producing_files
{
    VerticalSlicesEngine _engine;

    void Establish() => _engine = new VerticalSlicesEngine(_codeGenerator, _logger, _outputResolver, _chronicleResolver);

    async Task Because() => await _engine.Process(_modules);

    [Fact] void should_still_invoke_code_generator_for_each_slice() => _codeGenerator.Received(1).Generate(Arg.Any<VerticalSlice>(), Arg.Any<CodeGenerationContext>(), Arg.Any<ArtifactRenderSet>());
}
