// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;

namespace Cratis.VerticalSlices.for_VerticalSlicesEngine.when_previewing;

public class with_a_module_with_a_slice : given.a_module_with_a_slice_producing_files
{
    VerticalSlicesEngine _engine;
    IEnumerable<RenderedArtifact> _result;

    void Establish() => _engine = new VerticalSlicesEngine(_codeGenerator, _logger, _outputResolver, _chronicleResolver);

    void Because() => _result = _engine.Preview(_modules);

    [Fact] void should_return_generated_files() => _result.ShouldContainOnly(_generatedFile);
}
