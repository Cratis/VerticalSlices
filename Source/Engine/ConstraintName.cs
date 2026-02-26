// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Concepts;

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents the name of a constraint.
/// </summary>
/// <param name="Value">The name value.</param>
public record ConstraintName(string Value) : ConceptAs<string>(Value)
{
    /// <summary>
    /// Represents an empty <see cref="ConstraintName"/>.
    /// </summary>
    public static readonly ConstraintName Empty = new(string.Empty);

    /// <summary>
    /// Implicitly converts a <see cref="ConstraintName"/> to a <see cref="string"/>.
    /// </summary>
    /// <param name="name">The <see cref="ConstraintName"/> to convert.</param>
    public static implicit operator string(ConstraintName name) => name.Value;

    /// <summary>
    /// Implicitly converts a <see cref="string"/> to a <see cref="ConstraintName"/>.
    /// </summary>
    /// <param name="name">The <see cref="string"/> to convert.</param>
    public static implicit operator ConstraintName(string name) => new(name);
}
