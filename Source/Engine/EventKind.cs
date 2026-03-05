// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices;

/// <summary>
/// Whether a domain event originates inside or outside the system boundary.
/// Mirrors Studio's EventKind distinction.
/// </summary>
public enum EventKind
{
    /// <summary>
    /// A domain event produced by this system.
    /// </summary>
    Internal = 0,

    /// <summary>
    /// An event arriving from an external system, consumed by a Translator slice.
    /// External events are not registered with Chronicle — only the internal events
    /// they are translated into are registered.
    /// </summary>
    External = 1
}
