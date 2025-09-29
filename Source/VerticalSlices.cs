// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents the vertical slices configuration.
/// </summary>
/// <param name="ProjectFile">The path to the project file.</param>
public record VerticalSlices(string ProjectFile)
{
    /// <summary> The name of the vertical slices configuration file. </summary>
    public const string FileName = ".vertical-slices.json";

    static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    /// <summary>
    /// Attempts to load the vertical slices configuration from the specified root directory.
    /// </summary>
    /// <param name="root">The root directory to load from.</param>
    /// <param name="slices">The loaded vertical slices configuration, if found.</param>
    /// <returns>True if the configuration was found and loaded; otherwise, false.</returns>
    public static bool TryGetFrom(string root, [NotNullWhen(true)] out VerticalSlices? slices)
    {
        var path = Path.Combine(root, FileName);
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            slices = JsonSerializer.Deserialize<VerticalSlices>(json)!;
            return true;
        }
        slices = null;
        return false;
    }

    /// <summary>
    /// Sets the current project file in the vertical slices configuration.
    /// </summary>
    /// <param name="root">The root directory to save to.</param>
    /// <param name="projectFile">The project file to set.</param>
    public static void SetCurrentProject(string root, string projectFile)
    {
        var path = Path.Combine(root, FileName);
        var slices = new VerticalSlices(projectFile);
        var json = JsonSerializer.Serialize(slices, _jsonOptions);
        File.WriteAllText(path, json);
    }
}
