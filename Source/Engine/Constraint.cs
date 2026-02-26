// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents a constraint.
/// </summary>
/// <param name="Name">The name of the constraint.</param>
/// <param name="Type">The type of the constraint.</param>
/// <param name="Properties">The properties the constraint applies to.</param>
public record Constraint(ConstraintName Name, ConstraintType Type, IEnumerable<PropertyName> Properties);
