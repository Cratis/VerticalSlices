// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.Output;

/// <summary>
/// An <see cref="ICodeOutput"/> that performs no operations.
/// Used when no output target is configured or when only preview is needed.
/// </summary>
public class NoOpCodeOutput : ICodeOutput
{
    /// <inheritdoc/>
    public Task Write(IEnumerable<RenderedArtifact> artifacts, CancellationToken ct = default) =>
        Task.CompletedTask;
}
