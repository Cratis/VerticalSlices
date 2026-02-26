// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;
using Cratis.VerticalSlices.CodeGeneration.Renderers;

namespace Cratis.VerticalSlices.CodeGeneration.SliceTypes;

/// <summary>
/// Generates code for Automation slices. An Automation slice reacts to events,
/// maintains a "todo list" view model, and dispatches commands to external systems.
/// Flow: EventType(s) → ReadModel (task list) → Command → EventType(s).
/// </summary>
public class AutomationCodeGenerator : ISliceTypeCodeGenerator
{
    /// <inheritdoc/>
    public string SliceType => VerticalSliceTypes.Automation;

    /// <inheritdoc/>
    public IEnumerable<GeneratedFile> Generate(VerticalSlice slice, CodeGenerationContext context, ArtifactRenderSet renderSet)
    {
        var files = new List<GeneratedFile>();

        foreach (var eventType in slice.Events)
        {
            var descriptor = EventTypeDescriptor.FromEventType(eventType);
            files.AddRange(renderSet.EventType.Render(descriptor, context));
        }

        foreach (var readModel in slice.ReadModels)
        {
            var descriptor = ReadModelDescriptor.FromReadModel(readModel, slice.Screen);
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
