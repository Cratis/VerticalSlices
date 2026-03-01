// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents a command.
/// </summary>
/// <param name="Name">The name of the command.</param>
/// <param name="Description">The description of the command.</param>
/// <param name="Properties">The properties of the command.</param>
/// <param name="EventSourceId">The name of the property that acts as the event source id.</param>
/// <param name="EventSourceIdStrategy">How the event source id is obtained — supplied by the caller or auto-generated.</param>
/// <param name="ProducedEvents">
/// The events this command produces. When null, the engine infers produced events
/// from the slice context (e.g. all events in a StateChange slice). When specified,
/// only the listed events are associated with this command.
/// </param>
public record Command(
    string Name,
    string Description,
    IEnumerable<Property> Properties,
    string EventSourceId,
    EventSourceIdStrategy EventSourceIdStrategy = EventSourceIdStrategy.Supplied,
    IEnumerable<ProducedEvent>? ProducedEvents = null);
