// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;
using Cratis.VerticalSlices.CodeGeneration.Output;

namespace Cratis.VerticalSlices.for_VerticalSlicesEngine.when_processing;

public class with_output : given.a_module_with_a_slice_producing_files
{
    ICodeOutput _output;
    VerticalSlicesEngine _engine;

    void Establish()
    {
        _output = Substitute.For<ICodeOutput>();
        _outputResolver.Resolve().Returns(_output);
        _engine = new VerticalSlicesEngine(_codeGenerator, _logger, _outputResolver, _chronicleResolver);
    }

    async Task Because() => await _engine.Process(_modules);

    [Fact] void should_write_generated_files_to_output() => _output.Received(1).Write(Arg.Is<IEnumerable<RenderedArtifact>>(f => f.Contains(_generatedFile)), Arg.Any<CancellationToken>());
}
