// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Concepts;

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents the name of a query.
/// </summary>
/// <param name="Value">The name value.</param>
public record QueryName(string Value) : ConceptAs<string>(Value)
{
    /// <summary>
    /// Represents an empty <see cref="QueryName"/>.
    /// </summary>
    public static readonly QueryName Empty = new(string.Empty);

    /// <summary>
    /// Implicitly converts a <see cref="QueryName"/> to a <see cref="string"/>.
    /// </summary>
    /// <param name="name">The <see cref="QueryName"/> to convert.</param>
    public static implicit operator string(QueryName name) => name.Value;

    /// <summary>
    /// Implicitly converts a <see cref="string"/> to a <see cref="QueryName"/>.
    /// </summary>
    /// <param name="name">The <see cref="string"/> to convert.</param>
    public static implicit operator QueryName(string name) => new(name);
}
