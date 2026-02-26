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
    public async Task Write(IEnumerable<GeneratedFile> files, CancellationToken ct = default)
    {
        foreach (var file in files)
        {
            ct.ThrowIfCancellationRequested();

            var fullPath = Path.Combine(outputRoot, file.RelativePath);
            var directory = Path.GetDirectoryName(fullPath);

            if (directory is not null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllTextAsync(fullPath, file.Content, ct);

            LogWroteFile(file.RelativePath, fullPath);
        }
    }

    [LoggerMessage(LogLevel.Debug, "Wrote generated file {RelativePath} to {FullPath}")]
    partial void LogWroteFile(string relativePath, string fullPath);
}
