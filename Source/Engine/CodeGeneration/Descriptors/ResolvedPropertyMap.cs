// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.Descriptors;

/// <summary>
/// Describes how a single event property gets its value from the command context.
/// This is the resolved form of the command-to-event property mapping, ready for
/// renderer consumption. Symmetric with <see cref="PropertyMapping"/> which
/// describes event-to-read-model mappings.
/// </summary>
/// <param name="EventPropertyName">The name of the event property being populated.</param>
/// <param name="Source">Where the value comes from.</param>
/// <param name="SourcePropertyName">
/// The command property that supplies the value when <see cref="Source"/> is
/// <see cref="ResolvedPropertyMapSource.FromCommand"/>. Null for other sources.
/// </param>
public record ResolvedPropertyMap(
    string EventPropertyName,
    ResolvedPropertyMapSource Source,
    string? SourcePropertyName = null);
