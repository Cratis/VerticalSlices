// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.Integration.when_processing_an_automation_module;

/// <summary>
/// End-to-end scenario: an Automation slice with an event, a read model, and a command is
/// fed to the engine. The generated C# files are written to disk and then compiled with
/// <c>dotnet build</c> to verify the generated code is valid.
/// </summary>
public class with_read_model_and_command : given.a_real_engine
{
    IEnumerable<Module> _modules;

    void Establish()
    {
        var taskCreatedEvent = new EventType(
            "TaskCreated",
            "A task was created",
            [
                new Property("TaskId", "string"),
                new Property("TaskName", "string")
            ]);

        var taskIdMappings = new[]
        {
            new EventPropertyMapping("TaskCreated", EventPropertyMappingKind.Set, "TaskId")
        };

        var taskNameMappings = new[]
        {
            new EventPropertyMapping("TaskCreated", EventPropertyMappingKind.Set, "TaskName")
        };

        var pendingTask = new ReadModel(
            "PendingTask",
            "A task pending completion",
            [
                new ReadModelProperty("TaskId", "string", taskIdMappings),
                new ReadModelProperty("TaskName", "string", taskNameMappings)
            ]);

        var completeTaskCommand = new Command(
            "CompleteTask",
            "Completes a task",
            [],
            "TaskId");

        var automationSlice = new VerticalSlice(
            "ReviewTask",
            VerticalSliceType.Automation,
            null,
            null,
            [completeTaskCommand],
            [pendingTask],
            [taskCreatedEvent]);

        var feature = new Feature("TaskManagement", [], [], [automationSlice]);
        _modules = [new Module("WorkManagement", [], [feature])];
    }

    async Task Because()
    {
        await _engine.Process(_modules, _output);
        _generatedFiles = _engine.Preview(_modules);
        AddGlobalUsingsFromGeneratedFiles();
        _buildExitCode = await RunDotnet("build");
    }

    [Fact] void should_generate_an_event_type_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("TaskCreated.cs")).ShouldBeTrue();

    [Fact] void should_generate_a_projection_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("PendingTask.cs")).ShouldBeTrue();

    [Fact] void should_generate_an_observable_query_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("AllPendingTasks.cs")).ShouldBeTrue();

    [Fact] void should_generate_a_command_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("CompleteTask.cs")).ShouldBeTrue();

    [Fact] void should_compile_successfully() => _buildExitCode.ShouldEqual(0);
}
