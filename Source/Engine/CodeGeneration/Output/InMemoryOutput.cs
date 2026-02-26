// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.Output;

/// <summary>
/// An <see cref="ICodeOutput"/> that collects generated files in memory for preview purposes.
/// No files are written to disk or any external target.
/// </summary>
public class InMemoryOutput : ICodeOutput
{
    readonly List<GeneratedFile> _files = [];

    /// <summary>
    /// Gets the generated files that have been collected.
    /// </summary>
    public IReadOnlyList<GeneratedFile> Files => _files;

    /// <inheritdoc/>
    public Task Write(IEnumerable<GeneratedFile> files, CancellationToken ct = default)
    {
        _files.AddRange(files);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Clears all collected files.
    /// </summary>
    public void Clear() => _files.Clear();
}
