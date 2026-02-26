// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.Chronicle;

/// <summary>
/// Defines the contract for registering vertical slice artifacts with a Chronicle instance.
/// Implementations determine the transport: HTTP, gRPC, or no-op.
/// </summary>
public interface IChronicleRegistration
{
    /// <summary>
    /// Registers event types with Chronicle.
    /// </summary>
    /// <param name="eventTypes">The event type descriptors to register.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task RegisterEventTypes(IEnumerable<EventTypeDescriptor> eventTypes, CancellationToken ct = default);

    /// <summary>
    /// Registers projections for read models with Chronicle.
    /// </summary>
    /// <param name="readModels">The read model descriptors whose projections to register.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task RegisterProjections(IEnumerable<ReadModelDescriptor> readModels, CancellationToken ct = default);

    /// <summary>
    /// Registers read model type definitions with Chronicle.
    /// </summary>
    /// <param name="readModels">The read model descriptors to register as types.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task RegisterReadModelTypes(IEnumerable<ReadModelDescriptor> readModels, CancellationToken ct = default);
}
