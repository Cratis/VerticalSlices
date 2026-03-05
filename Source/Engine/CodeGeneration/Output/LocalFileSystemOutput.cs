// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace Cratis.VerticalSlices.CodeGeneration.Output;

/// <summary>
/// An <see cref="ICodeOutput"/> that writes generated files to a local directory on disk.
/// </summary>
/// <param name="outputRoot">The root directory to write files into.</param>
/// <param name="logger">The logger.</param>
public partial class LocalFileSystemOutput(string outputRoot, ILogger<LocalFileSystemOutput> logger) : ICodeOutput
{
    /// <inheritdoc/>
    public async Task Write(IEnumerable<RenderedArtifact> artifacts, CancellationToken ct = default)
    {
        foreach (var artifact in artifacts)
        {
            ct.ThrowIfCancellationRequested();

            var fullPath = Path.Combine(outputRoot, artifact.ArtifactPath);
            var directory = Path.GetDirectoryName(fullPath);

            if (directory is not null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllTextAsync(fullPath, artifact.Content, ct);

            LogWroteArtifact(artifact.ArtifactPath, fullPath);
        }
    }

    [LoggerMessage(LogLevel.Debug, "Wrote rendered artifact {ArtifactPath} to {FullPath}")]
    partial void LogWroteArtifact(string artifactPath, string fullPath);
}
