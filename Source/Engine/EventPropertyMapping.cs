// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices;

/// <summary>
/// Describes how a read model property is populated from a specific event type.
/// </summary>
/// <param name="EventTypeName">The name of the event type that drives this mapping.</param>
/// <param name="Kind">The kind of mapping operation to apply.</param>
/// <param name="SourcePropertyName">
/// The event property to read from, when the mapping kind reads a source value
/// (e.g. <see cref="EventPropertyMappingKind.Set"/>, <see cref="EventPropertyMappingKind.Add"/>,
/// <see cref="EventPropertyMappingKind.Subtract"/>). Null for kinds that have no source property
/// such as <see cref="EventPropertyMappingKind.Count"/> or <see cref="EventPropertyMappingKind.FromEventSourceId"/>.
/// </param>
public record EventPropertyMapping(
    string EventTypeName,
    EventPropertyMappingKind Kind,
    string? SourcePropertyName = null);
