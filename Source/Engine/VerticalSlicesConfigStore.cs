// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Cratis.VerticalSlices;

/// <summary>
/// Provides file-based persistence for <see cref="VerticalSlices"/> configuration.
/// Separates the I/O concerns from the pure data record.
/// </summary>
public static class VerticalSlicesConfigStore
{
    static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    /// <summary>
    /// Attempts to load the vertical slices configuration from the specified root directory.
    /// </summary>
    /// <param name="root">The root directory to load from.</param>
    /// <param name="slices">The loaded vertical slices configuration, if found.</param>
    /// <returns><see langword="true"/> if the configuration was found and loaded; otherwise, <see langword="false"/>.</returns>
    public static bool TryGetFrom(string root, [NotNullWhen(true)] out VerticalSlices? slices)
    {
        var path = Path.Combine(root, VerticalSlices.FileName);
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
    /// Persists the vertical slices configuration to the specified root directory.
    /// </summary>
    /// <param name="root">The root directory to save to.</param>
    /// <param name="projectFile">The project file path to store in the configuration.</param>
    public static void SetCurrentProject(string root, string projectFile)
    {
        var path = Path.Combine(root, VerticalSlices.FileName);
        var slices = new VerticalSlices(projectFile);
        var json = JsonSerializer.Serialize(slices, _jsonOptions);
        File.WriteAllText(path, json);
    }
}
