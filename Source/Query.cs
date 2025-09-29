// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents a query.
/// </summary>
/// <param name="Name">The name of the query.</param>
/// <param name="ReadModel">The read model the query is for.</param>
/// <param name="Properties">The properties of the query.</param>
public record Query(string Name, string ReadModel, IEnumerable<Property> Properties);
