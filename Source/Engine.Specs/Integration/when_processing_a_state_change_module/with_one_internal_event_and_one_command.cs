// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.Integration.when_processing_a_state_change_module;

/// <summary>
/// End-to-end scenario: a StateChange slice with one internal event and one command is fed
/// to the engine, the generated C# files are written to disk and then compiled with
/// <c>dotnet build</c> to verify the generated code is valid.
/// </summary>
public class with_one_internal_event_and_one_command : given.a_real_engine
{
    IEnumerable<Module> _modules;

    void Establish()
    {
        var internalEvent = new EventType(
            "EmployeeRegistered",
            "An employee was registered",
            [
                new Property("EmployeeId", "string"),
                new Property("FirstName", "string"),
                new Property("LastName", "string")
            ]);

        var command = new Command(
            "RegisterEmployee",
            "Registers a new employee",
            [
                new Property("FirstName", "string"),
                new Property("LastName", "string")
            ],
            "EmployeeId");

        var slice = new VerticalSlice(
            "RegisterEmployee",
            VerticalSliceType.StateChange,
            null,
            null,
            [command],
            [],
            [internalEvent]);

        var feature = new Feature("Registration", [], [], [slice]);
        _modules = [new Module("HumanResources", [], [feature])];
    }

    async Task Because()
    {
        await _engine.Process(_modules, _output);
        _generatedFiles = _engine.Preview(_modules);
        _buildExitCode = await RunDotnet("build");
    }

    [Fact] void should_generate_an_event_type_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("EmployeeRegistered.cs")).ShouldBeTrue();

    [Fact] void should_generate_a_command_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("RegisterEmployee.cs")).ShouldBeTrue();

    [Fact] void should_compile_successfully() => _buildExitCode.ShouldEqual(0);
}
