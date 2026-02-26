// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Concepts;

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents the name of a read model.
/// </summary>
/// <param name="Value">The name value.</param>
public record ReadModelName(string Value) : ConceptAs<string>(Value)
{
    /// <summary>
    /// Represents an empty <see cref="ReadModelName"/>.
    /// </summary>
    public static readonly ReadModelName Empty = new(string.Empty);

    /// <summary>
    /// Implicitly converts a <see cref="string"/> to a <see cref="ReadModelName"/>.
    /// </summary>
    /// <param name="name">The <see cref="string"/> to convert.</param>
    public static implicit operator ReadModelName(string name) => new(name);
}
