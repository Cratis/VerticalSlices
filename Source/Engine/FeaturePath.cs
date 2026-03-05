// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents the path through the feature hierarchy from a module level down to a specific
/// feature depth. The hierarchy follows Module → Feature → SubFeature, where this type captures
/// the feature segments of that path.
/// </summary>
/// <param name="Segments">The ordered feature name segments from root feature down.</param>
public record FeaturePath(IReadOnlyList<string> Segments)
{
    /// <summary>
    /// Gets an empty feature path, representing the module level with no feature nesting.
    /// </summary>
    public static FeaturePath Empty { get; } = new([]);

    /// <summary>
    /// Gets a value indicating whether this path has no segments.
    /// </summary>
    public bool IsEmpty => Segments.Count == 0;

    /// <summary>
    /// Creates a new <see cref="FeaturePath"/> by appending a feature segment.
    /// Used when traversing deeper into the feature hierarchy.
    /// </summary>
    /// <param name="segment">The feature name to append.</param>
    /// <returns>A new <see cref="FeaturePath"/> with the segment appended.</returns>
    public FeaturePath Append(string segment) => new([.. Segments, segment]);
}
