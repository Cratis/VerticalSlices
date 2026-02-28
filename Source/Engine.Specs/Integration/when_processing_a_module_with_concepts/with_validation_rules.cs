// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.Integration.when_processing_a_module_with_concepts;

/// <summary>
/// End-to-end scenario: a module with a concept that carries MinimumLength and MaximumLength
/// validation rules is fed to the engine. The renderer must produce both the concept record
/// file and a companion ConceptValidator class file. Both must compile successfully against
/// the <c>Cratis.Arc.Core</c> package (which provides <c>ConceptValidator&lt;T&gt;</c>).
/// </summary>
public class with_validation_rules : given.a_real_engine
{
    IEnumerable<Module> _modules;

    void Establish()
    {
        var concept = new Concept(
            "EmployeeName",
            "string",
            "An employee's full name",
            [
                new ConceptValidationRule(ConceptValidationRuleType.NotEmpty),
                new ConceptValidationRule(ConceptValidationRuleType.MinimumLength, "2"),
                new ConceptValidationRule(ConceptValidationRuleType.MaximumLength, "200")
            ]);

        _modules = [new Module("HumanResources", [concept], [])];
    }

    async Task Because()
    {
        await _engine.Process(_modules, _output);
        _generatedFiles = _engine.Preview(_modules);
        _buildExitCode = await RunDotnet("build");
    }

    [Fact] void should_generate_concept_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("EmployeeName.cs")).ShouldBeTrue();

    [Fact] void should_generate_validator_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("EmployeeNameValidator.cs")).ShouldBeTrue();

    [Fact] void should_compile_successfully() => _buildExitCode.ShouldEqual(0);
}
