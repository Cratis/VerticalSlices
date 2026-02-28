// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;

namespace Cratis.VerticalSlices.for_VerticalSlicesEngine.when_previewing;

/// <summary>
/// Verifies that the engine includes files generated from feature-level concepts in its preview output.
/// </summary>
public class with_feature_level_concepts : given.all_dependencies
{
    VerticalSlicesEngine _engine;
    IEnumerable<Module> _modules;
    IEnumerable<RenderedArtifact> _result;

    void Establish()
    {
        _engine = new VerticalSlicesEngine(_codeGenerator, _logger);

        var concept = new Concept("EmployeeName", "string", "An employee name", []);
        var feature = new Feature("Registration", [concept], [], []);
        _modules = [new Module("HumanResources", [], [feature])];
    }

    void Because() => _result = _engine.Preview(_modules);

    [Fact] void should_include_concept_file() => _result.Any(f => f.ArtifactPath.EndsWith("EmployeeName.cs")).ShouldBeTrue();
}
