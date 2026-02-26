// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.Descriptors;

/// <summary>
/// Describes the structure of a Chronicle event type, independent of how it will be rendered to code.
/// </summary>
/// <param name="Name">The event type name.</param>
/// <param name="Description">A human-readable description of the event.</param>
/// <param name="Properties">The properties carried by this event.</param>
public record EventTypeDescriptor(string Name, string Description, IEnumerable<Property> Properties)
{
    /// <summary>
    /// Creates an <see cref="EventTypeDescriptor"/> from an <see cref="EventType"/> domain model.
    /// </summary>
    /// <param name="eventType">The event type to describe.</param>
    /// <returns>A new <see cref="EventTypeDescriptor"/>.</returns>
    public static EventTypeDescriptor FromEventType(EventType eventType) =>
        new(eventType.Name, eventType.Description, eventType.Properties);
}
