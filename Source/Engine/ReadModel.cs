// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents a read model.
/// </summary>
/// <param name="Name">The name of the read model.</param>
/// <param name="Description">The description of the read model.</param>
/// <param name="Properties">The properties of the read model.</param>
public record ReadModel(string Name, string Description, IEnumerable<Property> Properties);
