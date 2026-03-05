// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.for_VerticalSlicesEngine.when_previewing;

public class with_a_module_with_a_slice : given.a_module_with_a_slice_producing_files
{
    VerticalSlicesEngine _engine;
    VerticalSlicesResult _result;

    void Establish() => _engine = new VerticalSlicesEngine(_codeGenerator, _advisor, _logger, _loggerFactory);

    void Because() => _result = _engine.Preview(_modules);

    [Fact] void should_return_generated_files() => _result.Artifacts.ShouldContainOnly(_generatedFile);
}
