// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;

namespace Cratis.VerticalSlices.for_VerticalSlicesEngine.when_previewing;

public class with_empty_modules : given.all_dependencies
{
    VerticalSlicesEngine _engine;
    VerticalSlicesResult _result;

    void Establish() => _engine = new VerticalSlicesEngine(_codeGenerator, _advisor, _logger, _outputResolver, _chronicleResolver);

    void Because() => _result = _engine.Preview([]);

    [Fact] void should_return_no_files() => _result.Artifacts.ShouldBeEmpty();
}
