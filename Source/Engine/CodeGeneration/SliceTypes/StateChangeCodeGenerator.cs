// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;
using Cratis.VerticalSlices.CodeGeneration.Renderers;

namespace Cratis.VerticalSlices.CodeGeneration.SliceTypes;

/// <summary>
/// Generates code for StateChange slices. A StateChange slice captures user intent
/// through a screen, maps it to a command, and produces domain events.
/// Flow: Screen → Command → EventType(s).
/// </summary>
public class StateChangeCodeGenerator : ISliceTypeCodeGenerator
{
    /// <inheritdoc/>
    public string SliceType => VerticalSliceTypes.StateChange;

    /// <inheritdoc/>
    public IEnumerable<GeneratedFile> Generate(VerticalSlice slice, CodeGenerationContext context, ArtifactRenderSet renderSet)
    {
        var files = new List<GeneratedFile>();

        foreach (var eventType in slice.Events)
        {
            var descriptor = EventTypeDescriptor.FromEventType(eventType);
            files.AddRange(renderSet.EventType.Render(descriptor, context));
        }

        foreach (var command in slice.Commands)
        {
            var descriptor = CommandDescriptor.FromCommand(command, slice.Events, slice.Screen);
            files.AddRange(renderSet.Command.Render(descriptor, context));
        }

        return files;
    }
}
