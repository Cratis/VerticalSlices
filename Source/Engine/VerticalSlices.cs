// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents the vertical slices configuration.
/// </summary>
/// <param name="ProjectFile">The path to the project file.</param>
public record VerticalSlices(string ProjectFile)
{
    /// <summary>The name of the vertical slices configuration file.</summary>
    public const string FileName = ".vertical-slices.json";
}
