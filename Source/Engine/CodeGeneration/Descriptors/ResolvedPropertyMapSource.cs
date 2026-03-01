// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.Descriptors;

/// <summary>
/// Defines the source of a resolved event property value in a command-to-event mapping.
/// </summary>
public enum ResolvedPropertyMapSource
{
    /// <summary>
    /// The value comes from a command property (by matching name or explicit mapping).
    /// </summary>
    FromCommand = 0,

    /// <summary>
    /// The value comes from the event source identifier.
    /// In controller rendering this maps to the <c>eventSourceId</c> local variable.
    /// In model-bound rendering this typically maps to <c>default!</c> since the
    /// framework supplies the event source id separately.
    /// </summary>
    FromEventSourceId = 1,

    /// <summary>
    /// The value has no source in the command and defaults to <c>default!</c> at runtime.
    /// </summary>
    ComputedDefault = 2
}
