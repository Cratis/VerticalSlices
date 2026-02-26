// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Concepts;

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents the name of a command.
/// </summary>
/// <param name="Value">The name value.</param>
public record CommandName(string Value) : ConceptAs<string>(Value)
{
    /// <summary>
    /// Represents an empty <see cref="CommandName"/>.
    /// </summary>
    public static readonly CommandName Empty = new(string.Empty);

    /// <summary>
    /// Implicitly converts a <see cref="string"/> to a <see cref="CommandName"/>.
    /// </summary>
    /// <param name="name">The <see cref="string"/> to convert.</param>
    public static implicit operator CommandName(string name) => new(name);
}
