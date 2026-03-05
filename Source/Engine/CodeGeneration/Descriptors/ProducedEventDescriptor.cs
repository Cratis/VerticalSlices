// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.Descriptors;

/// <summary>
/// Describes a produced event together with its fully resolved property mappings.
/// This is the descriptor-layer representation of a command producing an event,
/// carrying both the event type schema and the resolved mapping of how each event
/// property gets its value from the command.
/// </summary>
/// <param name="EventType">The event type descriptor with its property schema.</param>
/// <param name="PropertyMaps">The resolved property mappings from command to event.</param>
public record ProducedEventDescriptor(
    EventTypeDescriptor EventType,
    IEnumerable<ResolvedPropertyMap> PropertyMaps);
