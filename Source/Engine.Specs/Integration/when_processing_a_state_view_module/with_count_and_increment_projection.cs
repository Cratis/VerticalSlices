// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.Integration.when_processing_a_state_view_module;

/// <summary>
/// End-to-end scenario: a task management projection that demonstrates the Count, Increment
/// and Decrement mapping kinds. The read model counts tasks created, increments when completed,
/// and decrements (re-opened tasks reduce the completed count).
/// </summary>
public class with_count_and_increment_projection : given.a_real_engine
{
    IEnumerable<Module> _modules;

    void Establish()
    {
        var taskCreated = new EventType(
            "TaskCreated",
            "A task was created",
            [
                new Property("TaskId", "string"),
                new Property("ProjectId", "string"),
                new Property("Title", "string")
            ]);

        var taskCompleted = new EventType(
            "TaskCompleted",
            "A task was completed",
            [new Property("TaskId", "string"), new Property("ProjectId", "string")]);

        var taskReopened = new EventType(
            "TaskReopened",
            "A task was reopened",
            [new Property("TaskId", "string"), new Property("ProjectId", "string")]);

        // StateChange slice so event type classes are generated
        var createTaskCommand = new Command(
            "CreateTask",
            "Creates a new task",
            [new Property("Title", "string")],
            "ProjectId");

        var stateChangeSlice = new VerticalSlice(
            "TaskManagement",
            VerticalSliceType.StateChange,
            null,
            null,
            [createTaskCommand],
            [],
            [taskCreated, taskCompleted, taskReopened]);

        var taskFeature = new Feature("Tasks", [], [], [stateChangeSlice]);

        // Read model: project-level task stats
        var projectIdMappings = new[]
        {
            new EventPropertyMapping("TaskCreated", EventPropertyMappingKind.Set, "ProjectId")
        };

        var taskCountMappings = new[]
        {
            new EventPropertyMapping("TaskCreated", EventPropertyMappingKind.Count)
        };

        var completedCountMappings = new[]
        {
            new EventPropertyMapping("TaskCompleted", EventPropertyMappingKind.Increment)
        };

        var reopenedCountMappings = new[]
        {
            new EventPropertyMapping("TaskReopened", EventPropertyMappingKind.Decrement)
        };

        var statsReadModel = new ReadModel(
            "ProjectTaskStats",
            "Task statistics per project",
            [
                new ReadModelProperty("ProjectId", "string", projectIdMappings),
                new ReadModelProperty("TotalTasks", "int", taskCountMappings),
                new ReadModelProperty("CompletedTasks", "int", completedCountMappings),
                new ReadModelProperty("ReopenedTasks", "int", reopenedCountMappings)
            ]);

        var stateViewSlice = new VerticalSlice(
            "TaskStats",
            VerticalSliceType.StateView,
            null,
            null,
            [],
            [statsReadModel],
            [taskCreated, taskCompleted, taskReopened]);

        var statsFeature = new Feature("TaskStats", [], [], [stateViewSlice]);

        _modules = [new Module("TaskManagement", [], [taskFeature, statsFeature])];
    }

    async Task Because()
    {
        await _engine.Process(_modules, _output);
        _generatedFiles = _engine.Preview(_modules);
        AddGlobalUsingsFromGeneratedFiles();
        _buildExitCode = await RunDotnet("build");
    }

    [Fact] void should_generate_projection_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("ProjectTaskStats.cs")).ShouldBeTrue();

    [Fact] void should_generate_observable_query_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("AllProjectTaskStatss.cs")).ShouldBeTrue();

    [Fact] void should_generate_all_three_event_type_files() =>
        new[] { "TaskCreated.cs", "TaskCompleted.cs", "TaskReopened.cs" }
            .All(name => _generatedFiles.Any(f => f.RelativePath.EndsWith(name)))
            .ShouldBeTrue();

    [Fact] void should_compile_successfully() => _buildExitCode.ShouldEqual(0);
}
