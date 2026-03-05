// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.Descriptors;

/// <summary>
/// Describes a join relationship that pulls data from events in a different stream.
/// </summary>
/// <param name="ForeignKeyProperty">The read model property that holds the foreign key to join on.</param>
/// <param name="SourceEvents">The event types from the joined stream and their property mappings.</param>
public record JoinRelationship(
    string ForeignKeyProperty,
    IEnumerable<JoinEventSource> SourceEvents);
