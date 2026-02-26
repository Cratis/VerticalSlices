// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.Descriptors;

/// <summary>
/// Describes an event type that feeds into a child relationship,
/// including how the event identifies which child it targets.
/// </summary>
/// <param name="EventTypeName">The name of the event type.</param>
/// <param name="KeyProperty">The event property used to match the child's key.</param>
public record ChildEventSource(string EventTypeName, string KeyProperty);
