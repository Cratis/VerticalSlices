// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.Descriptors;

/// <summary>
/// Describes the structure of a read model with its full projection capabilities,
/// independent of how the projection will be rendered to code.
/// Supports property mappings, children, joins, removal, every-event mappings,
/// and projection behavior flags.
/// </summary>
/// <param name="Name">The read model name.</param>
/// <param name="Description">A human-readable description of the read model.</param>
/// <param name="Properties">The properties with their event mappings.</param>
/// <param name="SourceEvents">The event types that feed into this projection.</param>
/// <param name="Children">Child collection relationships with nested projections.</param>
/// <param name="Joins">Cross-stream join relationships.</param>
/// <param name="Removal">Describes when the entire read model instance is removed.</param>
/// <param name="EveryEventMappings">Mappings that apply on every event regardless of type.</param>
/// <param name="IsPassive">Whether this is a passive (in-memory, on-demand) projection.</param>
/// <param name="IsNotRewindable">Whether this projection is forward-only and cannot be rewound.</param>
/// <param name="EventSequence">A custom event sequence name, if not using the default.</param>
public record ReadModelDescriptor(
    string Name,
    string Description,
    IEnumerable<ReadModelPropertyDescriptor> Properties,
    IEnumerable<EventTypeDescriptor> SourceEvents,
    IEnumerable<ChildRelationship>? Children = null,
    IEnumerable<JoinRelationship>? Joins = null,
    RemovalDescriptor? Removal = null,
    IEnumerable<EveryEventMapping>? EveryEventMappings = null,
    bool IsPassive = false,
    bool IsNotRewindable = false,
    string? EventSequence = null)
{
    /// <summary>
    /// Creates a <see cref="ReadModelDescriptor"/> from a <see cref="ReadModel"/>,
    /// using the supplied available events together with explicit per-property mappings
    /// already defined on the read model properties.
    /// </summary>
    /// <param name="readModel">The read model to describe.</param>
    /// <param name="availableEvents">
    /// All event types known at the containing slice level. Only those referenced by
    /// at least one property mapping are included in <see cref="SourceEvents"/>.
    /// </param>
    /// <param name="screen">The optional screen whose fields contribute display metadata.</param>
    /// <returns>A new <see cref="ReadModelDescriptor"/>.</returns>
    public static ReadModelDescriptor FromReadModel(ReadModel readModel, IEnumerable<EventType> availableEvents, Screen? screen = null)
    {
        var eventList = availableEvents.ToList();
        var screenFields = screen?.Fields.ToList() ?? [];

        var referencedEventNames = readModel.Properties
            .SelectMany(p => p.Mappings)
            .Select(m => m.EventTypeName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var eventDescriptors = eventList
            .Where(e => referencedEventNames.Contains(e.Name))
            .Select(e => EventTypeDescriptor.FromEventType(e, ConceptScope.Empty));

        var properties = readModel.Properties.Select((property, index) =>
        {
            var mappings = property.Mappings.Select(m =>
            {
                var kind = (PropertyMappingKind)(int)m.Kind;
                var isContextMapping = kind is PropertyMappingKind.SetFromContext;

                return new PropertyMapping(
                    m.EventTypeName,
                    kind,
                    EventPropertyName: isContextMapping ? null : m.SourcePropertyName,
                    ContextProperty: isContextMapping ? m.SourcePropertyName : null);
            });

            var matchingField = screenFields
                .FirstOrDefault(f => f.Name.Equals(property.Name, StringComparison.OrdinalIgnoreCase));

            return new ReadModelPropertyDescriptor(
                property.Name,
                property.Type,
                index == 0,
                mappings,
                matchingField?.FieldType,
                matchingField?.Label);
        });

        return new ReadModelDescriptor(readModel.Name, readModel.Description, properties, eventDescriptors);
    }
}
