// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents a command.
/// </summary>
/// <param name="Name">The name of the command.</param>
/// <param name="Description">The description of the command.</param>
/// <param name="Properties">The properties of the command.</param>
public record Command(string Name, string Description, IEnumerable<Property> Properties);
