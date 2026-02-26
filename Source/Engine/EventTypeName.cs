// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Concepts;

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents the name of an event type.
/// </summary>
/// <param name="Value">The name value.</param>
public record EventTypeName(string Value) : ConceptAs<string>(Value)
{
    /// <summary>
    /// Represents an empty <see cref="EventTypeName"/>.
    /// </summary>
    public static readonly EventTypeName Empty = new(string.Empty);

    /// <summary>
    /// Implicitly converts an <see cref="EventTypeName"/> to a <see cref="string"/>.
    /// </summary>
    /// <param name="name">The <see cref="EventTypeName"/> to convert.</param>
    public static implicit operator string(EventTypeName name) => name.Value;

    /// <summary>
    /// Implicitly converts a <see cref="string"/> to an <see cref="EventTypeName"/>.
    /// </summary>
    /// <param name="name">The <see cref="string"/> to convert.</param>
    public static implicit operator EventTypeName(string name) => new(name);
}
