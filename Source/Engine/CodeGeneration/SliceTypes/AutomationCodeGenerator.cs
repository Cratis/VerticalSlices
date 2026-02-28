// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;
using Cratis.VerticalSlices.CodeGeneration.Renderers;

namespace Cratis.VerticalSlices.CodeGeneration.SliceTypes;

/// <summary>
/// Generates code for Automation slices. An Automation slice reacts to events,
/// maintains a "todo list" view model, and dispatches commands to external systems.
/// The <see cref="VerticalSlice.Events"/> list contains only events PRODUCED by the
/// automation's command. Consumed/input events that trigger the read model are
/// referenced by name in the read model property mappings and are not listed here.
/// Event type files are therefore generated only when the slice also has commands.
/// Flow: EventType(s) [consumed, not listed] → ReadModel (task list) → Command → EventType(s) [produced, listed + generated].
/// </summary>
public class AutomationCodeGenerator : ISliceTypeCodeGenerator
{
    /// <inheritdoc/>
    public VerticalSliceType SliceType => VerticalSliceType.Automation;

    /// <inheritdoc/>
    public IEnumerable<GeneratedFile> Generate(VerticalSlice slice, CodeGenerationContext context, ArtifactRenderSet renderSet)
    {
        var files = new List<GeneratedFile>();

        if (slice.Commands.Any())
        {
            foreach (var eventType in slice.Events)
            {
                var descriptor = EventTypeDescriptor.FromEventType(eventType);
                files.AddRange(renderSet.EventType.Render(descriptor, context));
            }
        }

        foreach (var readModel in slice.ReadModels)
        {
            var descriptor = ReadModelDescriptor.FromReadModel(readModel, slice.Events, slice.Screen);
            files.AddRange(renderSet.ReadModel.Render(descriptor, context));
        }

        foreach (var command in slice.Commands)
        {
            var descriptor = CommandDescriptor.FromCommand(command, [], slice.Screen);
            files.AddRange(renderSet.Command.Render(descriptor, context));
        }

        return files;
    }
}
