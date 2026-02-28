// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.Descriptors;

/// <summary>
/// Describes the structure of a command, including the events it produces,
/// independent of how it will be rendered to code.
/// </summary>
/// <param name="Name">The command name.</param>
/// <param name="Description">A human-readable description of the command.</param>
/// <param name="Properties">The command's input properties with optional screen field metadata.</param>
/// <param name="ProducedEvents">The event types this command produces when handled.</param>
/// <param name="EventSourceId">The name of the property that acts as the event source id.</param>
public record CommandDescriptor(
    string Name,
    string Description,
    IEnumerable<CommandPropertyDescriptor> Properties,
    IEnumerable<EventTypeDescriptor> ProducedEvents,
    string EventSourceId)
{
    /// <summary>
    /// Creates a <see cref="CommandDescriptor"/> from a <see cref="Command"/> and its produced events,
    /// matching screen fields to command properties by name.
    /// </summary>
    /// <param name="command">The command to describe.</param>
    /// <param name="producedEvents">The events this command produces.</param>
    /// <param name="screen">The optional screen whose fields map to command properties.</param>
    /// <returns>A new <see cref="CommandDescriptor"/>.</returns>
    public static CommandDescriptor FromCommand(Command command, IEnumerable<EventType> producedEvents, Screen? screen = null)
    {
        var screenFields = screen?.Fields.ToList() ?? [];

        var properties = command.Properties.Select(property =>
        {
            var matchingField = screenFields
                .FirstOrDefault(f => f.Name.Equals(property.Name, StringComparison.OrdinalIgnoreCase));

            return new CommandPropertyDescriptor(
                property.Name,
                property.Type,
                matchingField?.FieldType,
                matchingField?.Label);
        });

        return new CommandDescriptor(
            command.Name,
            command.Description,
            properties,
            producedEvents.Select(EventTypeDescriptor.FromEventType),
            command.EventSourceId);
    }
}
