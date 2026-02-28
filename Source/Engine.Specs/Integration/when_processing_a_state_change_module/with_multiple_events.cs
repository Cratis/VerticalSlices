// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.Integration.when_processing_a_state_change_module;

/// <summary>
/// End-to-end scenario: a StateChange slice whose command produces two separate events is
/// fed to the engine. The generated command file must compile with both event types referenced
/// in the Handle method stubs, and both event files must also compile.
/// </summary>
public class with_multiple_events : given.a_real_engine
{
    IEnumerable<Module> _modules;

    void Establish()
    {
        var projectCreatedEvent = new EventType(
            "ProjectCreated",
            "A project was created",
            [
                new Property("ProjectId", "string"),
                new Property("Name", "string")
            ]);

        var ownerAssignedEvent = new EventType(
            "ProjectOwnerAssigned",
            "An owner was assigned to a project",
            [
                new Property("ProjectId", "string"),
                new Property("OwnerId", "string")
            ]);

        var command = new Command(
            "CreateProject",
            "Creates a new project and assigns an owner",
            [
                new Property("Name", "string"),
                new Property("OwnerId", "string")
            ],
            "ProjectId");

        var slice = new VerticalSlice(
            "CreateProject",
            VerticalSliceType.StateChange,
            null,
            null,
            [command],
            [],
            [projectCreatedEvent, ownerAssignedEvent]);

        var feature = new Feature("Projects", [], [], [slice]);
        _modules = [new Module("ProjectManagement", [], [feature])];
    }

    async Task Because()
    {
        await _engine.Process(_modules, _output);
        _generatedFiles = _engine.Preview(_modules);
        _buildExitCode = await RunDotnet("build");
    }

    [Fact] void should_generate_first_event_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("ProjectCreated.cs")).ShouldBeTrue();

    [Fact] void should_generate_second_event_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("ProjectOwnerAssigned.cs")).ShouldBeTrue();

    [Fact] void should_generate_command_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("CreateProject.cs")).ShouldBeTrue();

    [Fact] void should_compile_successfully() => _buildExitCode.ShouldEqual(0);
}
