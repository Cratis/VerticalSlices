// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.Descriptors;

/// <summary>
/// Defines the kind of mapping operation between an event property and a read model property.
/// </summary>
public enum PropertyMappingKind
{
    /// <summary>
    /// Sets the property value from an event property (simple copy).
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
    /// Maps the property from the event source identifier.
    /// </summary>
    FromEventSourceId = 8
}
