// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;
using Cratis.VerticalSlices.CodeGeneration.Renderers;

namespace Cratis.VerticalSlices.CodeGeneration.SliceTypes;

/// <summary>
/// Generates code for StateChange slices. A StateChange slice captures user intent
/// through a screen, maps it to a command, and produces domain events.
/// When commands are present, only event types that are explicitly produced by at least
/// one command receive generated code. When no commands exist, all events are rendered.
/// Flow: Screen → Command → EventType(s).
/// </summary>
public class StateChangeCodeGenerator : ISliceTypeCodeGenerator
{
    /// <inheritdoc/>
    public VerticalSliceType SliceType => VerticalSliceType.StateChange;

    /// <inheritdoc/>
    public IEnumerable<RenderedArtifact> Generate(VerticalSlice slice, CodeGenerationContext context, ArtifactRenderSet renderSet)
    {
        var artifacts = new List<RenderedArtifact>();

        var commandedEventNames = slice.Commands
            .SelectMany(cmd =>
                cmd.ProducedEvents?.Select(pe => pe.EventTypeName)
                ?? slice.Events.Select(e => e.Name))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var eventsToRender = commandedEventNames.Count > 0
            ? slice.Events.Where(e => commandedEventNames.Contains(e.Name))
            : slice.Events;

        foreach (var eventType in eventsToRender)
        {
            var descriptor = EventTypeDescriptor.FromEventType(eventType, context.Concepts);
            artifacts.AddRange(renderSet.EventType.Render(descriptor, context));
        }

        foreach (var command in slice.Commands)
        {
            var descriptor = CommandDescriptor.FromCommand(command, slice.Events, slice.Screen, context.Concepts);
            artifacts.AddRange(renderSet.Command.Render(descriptor, context));
        }

        return artifacts;
    }
}
