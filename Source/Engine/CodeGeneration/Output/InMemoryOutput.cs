// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.Output;

/// <summary>
/// An <see cref="ICodeOutput"/> that collects generated files in memory for preview purposes.
/// No files are written to disk or any external target.
/// </summary>
public class InMemoryOutput : ICodeOutput
{
    readonly List<RenderedArtifact> _artifacts = [];

    /// <summary>
    /// Gets the rendered artifacts that have been collected.
    /// </summary>
    public IReadOnlyList<RenderedArtifact> Artifacts => _artifacts;

    /// <inheritdoc/>
    public Task Write(IEnumerable<RenderedArtifact> artifacts, CancellationToken ct = default)
    {
        _artifacts.AddRange(artifacts);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Clears all collected files.
    /// </summary>
    public void Clear() => _artifacts.Clear();
}
