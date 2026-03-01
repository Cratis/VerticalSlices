// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices;

/// <summary>
/// Declares that a command produces a specific event type.
/// When specified on <see cref="Command.ProducedEvents"/>, only the listed events
/// are associated with the command during code generation. When not specified
/// (null), the engine infers produced events from the slice context.
/// </summary>
/// <param name="EventTypeName">The name of the event type this command produces.</param>
public record ProducedEvent(string EventTypeName);
