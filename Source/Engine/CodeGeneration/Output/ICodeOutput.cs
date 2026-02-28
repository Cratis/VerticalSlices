// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.Output;

/// <summary>
/// Defines where generated code files are written to.
/// Implementations determine the target: local file system, Git repository, in-memory preview, etc.
/// </summary>
public interface ICodeOutput
{
    /// <summary>
    /// Writes the generated files to the output target.
    /// </summary>
    /// <param name="artifacts">The rendered artifacts to write.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task Write(IEnumerable<RenderedArtifact> artifacts, CancellationToken ct = default);
}
