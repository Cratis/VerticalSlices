// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents a read model. A read model acts as both the projection of events and the query
/// surface. Each property carries explicit <see cref="EventPropertyMapping"/> entries that
/// describe how it is populated from domain events held at the containing <see cref="VerticalSlice"/> level.
/// </summary>
/// <param name="Name">The name of the read model.</param>
/// <param name="Description">The description of the read model.</param>
/// <param name="Properties">The properties with their explicit event-to-property mappings.</param>
public record ReadModel(string Name, string Description, IEnumerable<ReadModelProperty> Properties);
