// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.Chronicle;

/// <summary>
/// An <see cref="IChronicleRegistration"/> that performs no operations.
/// Used when no Chronicle instance is available or when only code generation is needed.
/// </summary>
public class NoOpChronicleRegistration : IChronicleRegistration
{
    /// <inheritdoc/>
    public Task RegisterEventTypes(IEnumerable<EventTypeDescriptor> eventTypes, CancellationToken ct = default) =>
        Task.CompletedTask;

    /// <inheritdoc/>
    public Task RegisterProjections(IEnumerable<ReadModelDescriptor> readModels, CancellationToken ct = default) =>
        Task.CompletedTask;

    /// <inheritdoc/>
    public Task RegisterReadModelTypes(IEnumerable<ReadModelDescriptor> readModels, CancellationToken ct = default) =>
        Task.CompletedTask;
}
