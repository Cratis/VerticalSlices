// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents a property of a read model together with explicit per-event mappings
/// that describe how the property value is derived from incoming events.
/// When <see cref="Mappings"/> is empty the property is treated as read-only metadata
/// (e.g. a computed or externally populated value).
/// </summary>
/// <param name="Name">The property name.</param>
/// <param name="Type">The property type.</param>
/// <param name="Mappings">The explicit event-to-property mappings for this property.</param>
public record ReadModelProperty(string Name, string Type, IEnumerable<EventPropertyMapping> Mappings);
