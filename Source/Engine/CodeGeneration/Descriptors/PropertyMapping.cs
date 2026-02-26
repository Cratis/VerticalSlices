// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.Descriptors;

/// <summary>
/// Describes a single mapping operation from an event to a read model property.
/// </summary>
/// <param name="EventTypeName">The name of the event type this mapping comes from.</param>
/// <param name="Kind">The kind of mapping operation.</param>
/// <param name="EventPropertyName">The event property name, when the mapping reads from a specific property.</param>
/// <param name="ContextProperty">The event context property name, when mapping from event metadata (e.g., Occurred).</param>
/// <param name="StaticValue">The static value to set, when the mapping kind is <see cref="PropertyMappingKind.StaticValue"/>.</param>
public record PropertyMapping(
    string EventTypeName,
    PropertyMappingKind Kind,
    string? EventPropertyName = null,
    string? ContextProperty = null,
    string? StaticValue = null);
