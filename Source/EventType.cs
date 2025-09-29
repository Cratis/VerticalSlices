// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents an event type.
/// </summary>
/// <param name="Name">The name of the event type.</param>
/// <param name="Description">The description of the event type.</param>
/// <param name="Properties">The properties of the event type.</param>
public record EventType(string Name, string Description, IEnumerable<Property> Properties);
