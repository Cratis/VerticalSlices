// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.Descriptors;

/// <summary>
/// Describes a mapping that applies to every event regardless of type,
/// typically used for tracking last-updated timestamps.
/// </summary>
/// <param name="TargetProperty">The read model property to update.</param>
/// <param name="ContextProperty">The event context property to read from (e.g., Occurred).</param>
public record EveryEventMapping(string TargetProperty, string ContextProperty);
