// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Concepts;

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents the name of a feature.
/// </summary>
/// <param name="Value">The name value.</param>
public record FeatureName(string Value) : ConceptAs<string>(Value)
{
    /// <summary>
    /// Represents an empty <see cref="FeatureName"/>.
    /// </summary>
    public static readonly FeatureName Empty = new(string.Empty);

    /// <summary>
    /// Implicitly converts a <see cref="FeatureName"/> to a <see cref="string"/>.
    /// </summary>
    /// <param name="name">The <see cref="FeatureName"/> to convert.</param>
    public static implicit operator string(FeatureName name) => name.Value;

    /// <summary>
    /// Implicitly converts a <see cref="string"/> to a <see cref="FeatureName"/>.
    /// </summary>
    /// <param name="name">The <see cref="string"/> to convert.</param>
    public static implicit operator FeatureName(string name) => new(name);
}
