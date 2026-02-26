// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Concepts;

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents the name of a property.
/// </summary>
/// <param name="Value">The name value.</param>
public record PropertyName(string Value) : ConceptAs<string>(Value)
{
    /// <summary>
    /// Represents an empty <see cref="PropertyName"/>.
    /// </summary>
    public static readonly PropertyName Empty = new(string.Empty);

    /// <summary>
    /// Implicitly converts a <see cref="PropertyName"/> to a <see cref="string"/>.
    /// </summary>
    /// <param name="name">The <see cref="PropertyName"/> to convert.</param>
    public static implicit operator string(PropertyName name) => name.Value;

    /// <summary>
    /// Implicitly converts a <see cref="string"/> to a <see cref="PropertyName"/>.
    /// </summary>
    /// <param name="name">The <see cref="string"/> to convert.</param>
    public static implicit operator PropertyName(string name) => new(name);
}
