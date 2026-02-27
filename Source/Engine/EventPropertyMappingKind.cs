// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices;

/// <summary>
/// Defines the kind of mapping operation between a source event property and a read model property.
/// Integer values are kept in sync with <see cref="CodeGeneration.Descriptors.PropertyMappingKind"/>
/// so the cast <c>(PropertyMappingKind)(int)kind</c> is always safe.
/// </summary>
public enum EventPropertyMappingKind
{
    /// <summary>
    /// Sets the property value directly from an event property.
    /// </summary>
    Set = 0,

    /// <summary>
    /// Adds the event property value to the current property value.
    /// </summary>
    Add = 1,

    /// <summary>
    /// Subtracts the event property value from the current property value.
    /// </summary>
    Subtract = 2,

    /// <summary>
    /// Counts the number of times the event occurs.
    /// </summary>
    Count = 3,

    /// <summary>
    /// Increments the property value by one when the event occurs.
    /// </summary>
    Increment = 4,

    /// <summary>
    /// Decrements the property value by one when the event occurs.
    /// </summary>
    Decrement = 5,

    /// <summary>
    /// Sets the property value from event context metadata (e.g., Occurred, SequenceNumber).
    /// </summary>
    SetFromContext = 6,

    /// <summary>
    /// Sets the property to a static constant value when the event occurs.
    /// </summary>
    StaticValue = 7,

    /// <summary>
    /// Sets the property value from the event source identifier.
    /// </summary>
    FromEventSourceId = 8
}
