// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.for_VerticalSlicesEngine.when_processing;

/// <summary>
/// Verifies that the engine writes concept files generated from feature-level concepts to the output.
/// </summary>
public class with_feature_level_concepts : given.all_dependencies
{
    VerticalSlicesEngine _engine;
    VerticalSlicesResult _result;
    IEnumerable<Module> _modules;

    void Establish()
    {
        _engine = new VerticalSlicesEngine(_codeGenerator, _advisor, _logger, _loggerFactory);

        var concept = new Concept("EmployeeName", "string", "An employee name", []);
        var feature = new Feature("Registration", [concept], [], []);
        _modules = [new Module("HumanResources", [], [feature])];
    }

    async Task Because() => _result = await _engine.Process(_modules);

    [Fact] void should_include_concept_file_in_result() =>
        _result.Artifacts.Any(a => a.ArtifactPath.EndsWith("EmployeeName.cs")).ShouldBeTrue();
}
