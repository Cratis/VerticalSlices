// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.for_VerticalSlicesEngine.when_processing;

public class with_output : given.a_module_with_a_slice_producing_files
{
    VerticalSlicesEngine _engine;
    VerticalSlicesResult _result;

    void Establish() => _engine = new VerticalSlicesEngine(_codeGenerator, _advisor, _logger, _loggerFactory);

    async Task Because() => _result = await _engine.Process(_modules);

    [Fact] void should_include_generated_file_in_result() => _result.Artifacts.ShouldContain(_generatedFile);
}
