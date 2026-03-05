// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.Descriptors;

/// <summary>
/// Describes when a read model or child item should be removed.
/// </summary>
/// <param name="EventTypeName">The event type that triggers removal.</param>
/// <param name="KeyProperty">The event property used to identify which item to remove, for child relationships.</param>
public record RemovalDescriptor(string EventTypeName, string? KeyProperty = null);
