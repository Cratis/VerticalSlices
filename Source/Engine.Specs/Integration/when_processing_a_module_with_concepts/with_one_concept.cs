// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.Integration.when_processing_a_module_with_concepts;

/// <summary>
/// End-to-end scenario: a module with a module-level concept and a feature-level concept
/// is fed to the engine. The generated C# files are written to disk and then compiled
/// with <c>dotnet build</c> to verify the generated code is valid.
/// </summary>
public class with_one_concept : given.a_real_engine
{
    IEnumerable<Module> _modules;

    void Establish()
    {
        var employeeIdConcept = new Concept(
            "EmployeeId",
            "Guid",
            "An employee identifier",
            []);

        var employeeNameConcept = new Concept(
            "EmployeeName",
            "string",
            "An employee name",
            []);

        var feature = new Feature("Registration", [employeeNameConcept], [], []);
        _modules = [new Module("HumanResources", [employeeIdConcept], [feature])];
    }

    async Task Because()
    {
        await _engine.Process(_modules, _output);
        _generatedFiles = _engine.Preview(_modules);
        _buildExitCode = await RunDotnet("build");
    }

    [Fact] void should_generate_module_level_concept_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("EmployeeId.cs")).ShouldBeTrue();

    [Fact] void should_generate_feature_level_concept_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("EmployeeName.cs")).ShouldBeTrue();

    [Fact] void should_compile_successfully() => _buildExitCode.ShouldEqual(0);
}
