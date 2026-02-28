// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.Integration.when_processing_a_state_view_module;

/// <summary>
/// End-to-end scenario: a StateView slice that contains two separate read models is fed to
/// the engine. Each read model produces a projection record and an AllXxx observable query
/// class. All four generated files must compile successfully in the same project.
/// </summary>
public class with_multiple_read_models : given.a_real_engine
{
    IEnumerable<Module> _modules;

    void Establish()
    {
        var employeeRegisteredEvent = new EventType(
            "EmployeeRegistered",
            "An employee was registered",
            [
                new Property("EmployeeId", "string"),
                new Property("DepartmentId", "string"),
                new Property("FullName", "string")
            ]);

        var employeeIdMappingForProfile = new[]
        {
            new EventPropertyMapping("EmployeeRegistered", EventPropertyMappingKind.Set, "EmployeeId")
        };

        var fullNameMappings = new[]
        {
            new EventPropertyMapping("EmployeeRegistered", EventPropertyMappingKind.Set, "FullName")
        };

        var employeeProfile = new ReadModel(
            "EmployeeProfile",
            "A summary profile for an employee",
            [
                new ReadModelProperty("EmployeeId", "string", employeeIdMappingForProfile),
                new ReadModelProperty("FullName", "string", fullNameMappings)
            ]);

        var employeeIdMappingForDept = new[]
        {
            new EventPropertyMapping("EmployeeRegistered", EventPropertyMappingKind.Set, "EmployeeId")
        };

        var departmentIdMappings = new[]
        {
            new EventPropertyMapping("EmployeeRegistered", EventPropertyMappingKind.Set, "DepartmentId")
        };

        var headcountEntry = new ReadModel(
            "DepartmentHeadcount",
            "Tracks headcount per department",
            [
                new ReadModelProperty("EmployeeId", "string", employeeIdMappingForDept),
                new ReadModelProperty("DepartmentId", "string", departmentIdMappings)
            ]);

        // A StateChange slice in a separate feature so that the EmployeeRegistered
        // event type class is generated and visible to the projection attributes.
        var registerCommand = new Command(
            "RegisterEmployee",
            "Registers a new employee",
            [
                new Property("DepartmentId", "string"),
                new Property("FullName", "string")
            ],
            "EmployeeId");

        var stateChangeSlice = new VerticalSlice(
            "Registration",
            VerticalSliceType.StateChange,
            null,
            null,
            [registerCommand],
            [],
            [employeeRegisteredEvent]);

        var registrationFeature = new Feature("Registration", [], [], [stateChangeSlice]);

        var stateViewSlice = new VerticalSlice(
            "PeopleReporting",
            VerticalSliceType.StateView,
            null,
            null,
            [],
            [employeeProfile, headcountEntry],
            [employeeRegisteredEvent]);

        var reportingFeature = new Feature("Reporting", [], [], [stateViewSlice]);
        _modules = [new Module("HumanResources", [], [registrationFeature, reportingFeature])];
    }

    async Task Because()
    {
        await _engine.Process(_modules, _output);
        _generatedFiles = _engine.Preview(_modules);
        AddGlobalUsingsFromGeneratedFiles();
        _buildExitCode = await RunDotnet("build");
    }

    [Fact] void should_generate_first_projection_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("EmployeeProfile.cs")).ShouldBeTrue();

    [Fact] void should_generate_first_observable_query_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("AllEmployeeProfiles.cs")).ShouldBeTrue();

    [Fact] void should_generate_second_projection_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("DepartmentHeadcount.cs")).ShouldBeTrue();

    [Fact] void should_generate_second_observable_query_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("AllDepartmentHeadcounts.cs")).ShouldBeTrue();

    [Fact] void should_compile_successfully() => _buildExitCode.ShouldEqual(0);
}
