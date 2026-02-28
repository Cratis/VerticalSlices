// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundReadModelRenderer.when_rendering;

/// <summary>
/// A read model with multiple properties each using distinct mapping kinds renders
/// the correct attribute for every kind: [SetFrom], [AddFrom], [SubtractFrom], [Count],
/// [Increment], [Decrement] on their respective properties.
/// </summary>
public class with_multiple_properties_with_different_mapping_kinds : given.a_context
{
    ModelBoundReadModelRenderer _renderer;
    ReadModelDescriptor _descriptor;
    string _projectionContent;

    void Establish()
    {
        _renderer = new ModelBoundReadModelRenderer();
        _descriptor = new ReadModelDescriptor(
            "ProjectStats",
            "Stats for a project",
            [
                new ReadModelPropertyDescriptor("ProjectId", "string", IsKey: true,
                [
                    new PropertyMapping("ProjectCreated", PropertyMappingKind.Set, "ProjectId")
                ]),
                new ReadModelPropertyDescriptor("TotalBudget", "decimal", IsKey: false,
                [
                    new PropertyMapping("BudgetAllocated", PropertyMappingKind.Add, "Amount")
                ]),
                new ReadModelPropertyDescriptor("SpentBudget", "decimal", IsKey: false,
                [
                    new PropertyMapping("ExpenseRecorded", PropertyMappingKind.Add, "Amount"),
                    new PropertyMapping("ExpenseReversed", PropertyMappingKind.Subtract, "Amount")
                ]),
                new ReadModelPropertyDescriptor("TaskCount", "int", IsKey: false,
                [
                    new PropertyMapping("TaskAdded", PropertyMappingKind.Count)
                ]),
                new ReadModelPropertyDescriptor("CompletedCount", "int", IsKey: false,
                [
                    new PropertyMapping("TaskCompleted", PropertyMappingKind.Increment)
                ]),
                new ReadModelPropertyDescriptor("ReopenedCount", "int", IsKey: false,
                [
                    new PropertyMapping("TaskReopened", PropertyMappingKind.Decrement)
                ])
            ],
            []);
    }

    void Because() => _projectionContent = _renderer.Render(_descriptor, _context)
        .Single(f => f.ArtifactPath.EndsWith("ProjectStats.cs")).Content;

    [Fact] void should_emit_from_event_on_project_created() => _projectionContent.ShouldContain("[FromEvent<ProjectCreated>]");
    [Fact] void should_emit_add_from_on_total_budget() => _projectionContent.ShouldContain("[AddFrom<BudgetAllocated>");
    [Fact] void should_emit_add_from_on_spent_budget() => _projectionContent.ShouldContain("[AddFrom<ExpenseRecorded>");
    [Fact] void should_emit_subtract_from_on_spent_budget() => _projectionContent.ShouldContain("[SubtractFrom<ExpenseReversed>");
    [Fact] void should_emit_count_on_task_count() => _projectionContent.ShouldContain("[Count<TaskAdded>]");
    [Fact] void should_emit_increment_on_completed_count() => _projectionContent.ShouldContain("[Increment<TaskCompleted>]");
    [Fact] void should_emit_decrement_on_reopened_count() => _projectionContent.ShouldContain("[Decrement<TaskReopened>]");
}
