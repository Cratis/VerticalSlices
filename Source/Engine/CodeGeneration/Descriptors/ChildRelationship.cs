// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.Descriptors;

/// <summary>
/// Describes a child collection relationship in a projection.
/// Children are nested collections identified by a key property,
/// with their own event-to-property mappings.
/// </summary>
/// <param name="PropertyName">The collection property name on the parent read model.</param>
/// <param name="ChildTypeName">The type name of the child record.</param>
/// <param name="KeyProperty">The property used to identify individual children.</param>
/// <param name="Properties">The properties of the child record with their mappings.</param>
/// <param name="SourceEvents">The events that create or update children.</param>
/// <param name="ParentKeyProperty">The event property referencing the parent, if applicable.</param>
/// <param name="Removal">Describes when a child is removed from the collection.</param>
/// <param name="Children">Nested child relationships within this child.</param>
public record ChildRelationship(
    string PropertyName,
    string ChildTypeName,
    string KeyProperty,
    IEnumerable<ReadModelPropertyDescriptor> Properties,
    IEnumerable<ChildEventSource> SourceEvents,
    string? ParentKeyProperty = null,
    RemovalDescriptor? Removal = null,
    IEnumerable<ChildRelationship>? Children = null);
