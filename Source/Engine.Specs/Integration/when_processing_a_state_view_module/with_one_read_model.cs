// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.Integration.when_processing_a_state_view_module;

/// <summary>
/// End-to-end scenario: a StateView slice with one read model (projection + observable query)
/// is fed to the engine, the generated C# files are written to disk and then compiled with
/// <c>dotnet build</c> to verify the generated code is valid.
/// </summary>
public class with_one_read_model : given.a_real_engine
{
    IEnumerable<Module> _modules;

    void Establish()
    {
        // --- event type shared between StateChange and StateView ---
        var employeeRegisteredEvent = new EventType(
            "EmployeeRegistered",
            "An employee was registered",
            [
                new Property("EmployeeId", "string"),
                new Property("FirstName", "string"),
                new Property("LastName", "string")
            ]);

        // --- Registration feature: StateChange produces the EmployeeRegistered event type file ---
        var registerEmployeeCommand = new Command(
            "RegisterEmployee",
            "Registers a new employee",
            [
                new Property("FirstName", "string"),
                new Property("LastName", "string")
            ],
            "EmployeeId");

        var registrationSlice = new VerticalSlice(
            "Registration",
            VerticalSliceType.StateChange,
            null,
            null,
            [registerEmployeeCommand],
            [],
            [employeeRegisteredEvent]);

        var registrationFeature = new Feature("Registration", [], [], [registrationSlice]);

        // --- EmployeeManagement feature: StateView consumes EmployeeRegistered ---
        var employeeIdMappings = new[]
        {
            new EventPropertyMapping("EmployeeRegistered", EventPropertyMappingKind.Set, "EmployeeId")
        };

        var firstNameMappings = new[]
        {
            new EventPropertyMapping("EmployeeRegistered", EventPropertyMappingKind.Set, "FirstName")
        };

        var lastNameMappings = new[]
        {
            new EventPropertyMapping("EmployeeRegistered", EventPropertyMappingKind.Set, "LastName")
        };

        var readModel = new ReadModel(
            "Employee",
            "Represents an employee",
            [
                new ReadModelProperty("EmployeeId", "string", employeeIdMappings),
                new ReadModelProperty("FirstName", "string", firstNameMappings),
                new ReadModelProperty("LastName", "string", lastNameMappings)
            ]);

        var stateViewSlice = new VerticalSlice(
            "Employees",
            VerticalSliceType.StateView,
            null,
            null,
            [],
            [readModel],
            [employeeRegisteredEvent]);

        var employeeManagementFeature = new Feature("EmployeeManagement", [], [], [stateViewSlice]);

        _modules = [new Module("HumanResources", [], [registrationFeature, employeeManagementFeature])];
    }

    async Task Because()
    {
        await _engine.Process(_modules, _output);
        _generatedFiles = _engine.Preview(_modules);
        AddGlobalUsingsFromGeneratedFiles();
        _buildExitCode = await RunDotnet("build");
    }

    [Fact] void should_generate_a_projection_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("Employee.cs")).ShouldBeTrue();

    [Fact] void should_generate_an_observable_query_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("AllEmployees.cs")).ShouldBeTrue();

    [Fact] void should_compile_successfully() => _buildExitCode.ShouldEqual(0);
}
