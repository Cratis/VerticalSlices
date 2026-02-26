// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Concepts;

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents a description.
/// </summary>
/// <param name="Value">The description value.</param>
public record Description(string Value) : ConceptAs<string>(Value)
{
    /// <summary>
    /// Represents an empty <see cref="Description"/>.
    /// </summary>
    public static readonly Description Empty = new(string.Empty);

    /// <summary>
    /// Implicitly converts a <see cref="string"/> to a <see cref="Description"/>.
    /// </summary>
    /// <param name="description">The <see cref="string"/> to convert.</param>
    public static implicit operator Description(string description) => new(description);
}
