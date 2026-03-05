// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.Descriptors;

/// <summary>
/// Describes an event type used in a join relationship, including which of its
/// properties map to the read model.
/// </summary>
/// <param name="EventTypeName">The name of the joined event type.</param>
/// <param name="PropertyMappings">The mappings from event properties to read model properties.</param>
public record JoinEventSource(string EventTypeName, IEnumerable<PropertyMapping> PropertyMappings);
