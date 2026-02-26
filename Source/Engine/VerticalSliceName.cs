// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Concepts;

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents the name of a vertical slice.
/// </summary>
/// <param name="Value">The name value.</param>
public record VerticalSliceName(string Value) : ConceptAs<string>(Value)
{
    /// <summary>
    /// Represents an empty <see cref="VerticalSliceName"/>.
    /// </summary>
    public static readonly VerticalSliceName Empty = new(string.Empty);

    /// <summary>
    /// Implicitly converts a <see cref="VerticalSliceName"/> to a <see cref="string"/>.
    /// </summary>
    /// <param name="name">The <see cref="VerticalSliceName"/> to convert.</param>
    public static implicit operator string(VerticalSliceName name) => name.Value;

    /// <summary>
    /// Implicitly converts a <see cref="string"/> to a <see cref="VerticalSliceName"/>.
    /// </summary>
    /// <param name="name">The <see cref="string"/> to convert.</param>
    public static implicit operator VerticalSliceName(string name) => new(name);
}
