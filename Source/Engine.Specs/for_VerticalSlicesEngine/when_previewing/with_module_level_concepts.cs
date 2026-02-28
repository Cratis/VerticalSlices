// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;

namespace Cratis.VerticalSlices.for_VerticalSlicesEngine.when_previewing;

/// <summary>
/// Verifies that the engine includes files generated from module-level concepts in its preview output.
/// </summary>
public class with_module_level_concepts : given.all_dependencies
{
    VerticalSlicesEngine _engine;
    IEnumerable<Module> _modules;
    IEnumerable<GeneratedFile> _result;

    void Establish()
    {
        _engine = new VerticalSlicesEngine(_codeGenerator, _logger);

        var concept = new Concept("EmployeeId", "Guid", "An employee identifier", []);
        _modules = [new Module("HumanResources", [concept], [])];
    }

    void Because() => _result = _engine.Preview(_modules);

    [Fact] void should_include_concept_file() => _result.Any(f => f.RelativePath.EndsWith("EmployeeId.cs")).ShouldBeTrue();
}
