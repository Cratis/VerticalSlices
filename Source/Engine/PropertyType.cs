// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Concepts;

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents the type of a property.
/// </summary>
/// <param name="Value">The type value.</param>
public record PropertyType(string Value) : ConceptAs<string>(Value)
{
    /// <summary>
    /// Represents an empty <see cref="PropertyType"/>.
    /// </summary>
    public static readonly PropertyType Empty = new(string.Empty);

    /// <summary>
    /// Implicitly converts a <see cref="string"/> to a <see cref="PropertyType"/>.
    /// </summary>
    /// <param name="type">The <see cref="string"/> to convert.</param>
    public static implicit operator PropertyType(string type) => new(type);
}
