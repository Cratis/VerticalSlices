// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound;

/// <summary>
/// Renders a read model descriptor as a Chronicle model-bound projection record that also
/// embeds the observable <c>GetAll</c> query method. Produces a single partial record
/// decorated with [ReadModel] and the relevant projection attributes.
/// </summary>
public class ModelBoundReadModelRenderer : IArtifactRenderer<ReadModelDescriptor>
{
    /// <inheritdoc/>
    public IEnumerable<RenderedArtifact> Render(ReadModelDescriptor descriptor, CodeGenerationContext context) =>
        [RenderProjection(descriptor, context)];

    /// <summary>
    /// Renders the projection record with Chronicle model-bound attributes and an embedded
    /// observable query method.
    /// </summary>
    /// <param name="descriptor">The read model descriptor.</param>
    /// <param name="context">The code generation context.</param>
    /// <returns>A rendered artifact for the projection.</returns>
    static RenderedArtifact RenderProjection(ReadModelDescriptor descriptor, CodeGenerationContext context)
    {
        var builder = new CSharpCodeBuilder(context)
            .Using(
                "Cratis.Arc.Queries.ModelBound",
                "Cratis.Chronicle.Events",
                "Cratis.Chronicle.Keys",
                "Cratis.Chronicle.Projections.ModelBound",
                "Cratis.Chronicle.ReadModels",
                "System.Reactive.Subjects")
            .Namespace(context.Namespace)
            .BlankLine();

        if (!string.IsNullOrWhiteSpace(descriptor.Description))
        {
            builder.Summary(descriptor.Description);
        }

        builder.Attribute("ReadModel");
        AppendTypeAttributes(builder, descriptor);

        var properties = descriptor.Properties.ToList();
        builder.BeginPrimaryConstructorParameters(descriptor.Name, isPartial: true);

        for (var i = 0; i < properties.Count; i++)
        {
            var property = properties[i];
            var isLast = i == properties.Count - 1;

            AppendPropertyAttributes(builder, property);
            builder.ConstructorParameter($"{property.Type} {property.Name}", isLast, openBody: isLast);

            if (!isLast)
            {
                builder.BlankLine();
            }
        }

        builder
            .Summary($"Gets all {descriptor.Name} instances as an observable subject.")
            .XmlParam("readModels", "The read models provider.")
            .XmlReturns($"An observable subject of all {descriptor.Name} instances.")
            .ExpressionMember(
                $"ISubject<{descriptor.Name}>",
                "GetAll",
                $"readModels.Watch<{descriptor.Name}>().ToObservableReadModel();",
                "IReadModels readModels",
                isStatic: true)
            .EndBlock();

        var artifactPath = Path.Combine(context.RelativePath, $"{descriptor.Name}.cs");
        return new RenderedArtifact(artifactPath, builder.ToString());
    }

    /// <summary>
    /// Appends record-level attributes such as [Passive], [NotRewindable], [FromEventSequence],
    /// [RemovedWith], and [FromEvent] for auto-mapped events.
    /// </summary>
    /// <param name="builder">The code builder.</param>
    /// <param name="descriptor">The read model descriptor.</param>
    static void AppendTypeAttributes(CSharpCodeBuilder builder, ReadModelDescriptor descriptor)
    {
        if (descriptor.IsPassive)
        {
            builder.Attribute("Passive");
        }

        if (descriptor.IsNotRewindable)
        {
            builder.Attribute("NotRewindable");
        }

        if (!string.IsNullOrWhiteSpace(descriptor.EventSequence))
        {
            builder.Attribute($"FromEventSequence(\"{descriptor.EventSequence}\")");
        }

        if (descriptor.Removal is not null)
        {
            builder.Attribute($"RemovedWith<{descriptor.Removal.EventTypeName}>");
        }

        // Emit [FromEvent<T>] for every event type that has at least one Set mapping
        // where the event property name matches the read model property name (auto-mapping).
        var autoMappedEventTypes = descriptor.Properties
            .SelectMany(p => p.Mappings
                .Where(m => m.Kind == PropertyMappingKind.Set && m.EventPropertyName == p.Name)
                .Select(m => m.EventTypeName))
            .Distinct();

        foreach (var eventTypeName in autoMappedEventTypes)
        {
            builder.Attribute($"FromEvent<{eventTypeName}>");
        }
    }

    /// <summary>
    /// Appends per-property attributes based on the property's mappings.
    /// Set mappings where the event property name matches the read model property name are
    /// handled by a class-level [FromEvent&lt;T&gt;] attribute and are skipped here.
    /// </summary>
    /// <param name="builder">The code builder.</param>
    /// <param name="property">The property descriptor.</param>
    static void AppendPropertyAttributes(CSharpCodeBuilder builder, ReadModelPropertyDescriptor property)
    {
        if (property.IsKey)
        {
            builder.Attribute("Key");
        }

        foreach (var mapping in property.Mappings)
        {
            // Skip Set mappings whose event property name matches the property name —
            // those are covered by the class-level [FromEvent<T>] attribute.
            if (mapping.Kind == PropertyMappingKind.Set && mapping.EventPropertyName == property.Name)
            {
                continue;
            }

            var attribute = mapping.Kind switch
            {
                PropertyMappingKind.Set =>
                    $"SetFrom<{mapping.EventTypeName}>(nameof({mapping.EventTypeName}.{mapping.EventPropertyName}))",

                PropertyMappingKind.Add =>
                    $"AddFrom<{mapping.EventTypeName}>(nameof({mapping.EventTypeName}.{mapping.EventPropertyName}))",

                PropertyMappingKind.Subtract =>
                    $"SubtractFrom<{mapping.EventTypeName}>(nameof({mapping.EventTypeName}.{mapping.EventPropertyName}))",

                PropertyMappingKind.Count =>
                    $"Count<{mapping.EventTypeName}>",

                PropertyMappingKind.Increment =>
                    $"Increment<{mapping.EventTypeName}>",

                PropertyMappingKind.Decrement =>
                    $"Decrement<{mapping.EventTypeName}>",

                PropertyMappingKind.SetFromContext when mapping.EventTypeName != "*" =>
                    $"SetFromContext<{mapping.EventTypeName}>(nameof(EventContext.{mapping.ContextProperty}))",

                PropertyMappingKind.StaticValue => null,
                PropertyMappingKind.FromEventSourceId => null,
                _ => null
            };

            if (attribute is not null)
            {
                builder.Attribute(attribute);
            }
        }

        var everyMappings = property.Mappings
            .Where(m => m.Kind is PropertyMappingKind.SetFromContext && m.EventTypeName == "*");

        foreach (var every in everyMappings)
        {
            builder.Attribute($"FromEvery(contextProperty: nameof(EventContext.{every.ContextProperty}))");
        }
    }
}
