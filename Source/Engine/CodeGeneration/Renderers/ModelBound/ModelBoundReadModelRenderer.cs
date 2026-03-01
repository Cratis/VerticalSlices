// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;
using Humanizer;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound;

/// <summary>
/// Renders a read model descriptor as a Chronicle model-bound projection record that also
/// embeds observable query methods. Produces a single partial record decorated with
/// [ReadModel] and the relevant projection attributes.
/// Query methods use <c>IMongoCollection&lt;T&gt;</c> with <c>.Observe()</c> returning
/// <c>ISubject&lt;IEnumerable&lt;T&gt;&gt;</c> for collection queries and
/// <c>ISubject&lt;T&gt;</c> for by-id queries.
/// </summary>
public class ModelBoundReadModelRenderer : IArtifactRenderer<ReadModelDescriptor>
{
    /// <inheritdoc/>
    public IEnumerable<RenderedArtifact> Render(ReadModelDescriptor descriptor, CodeGenerationContext context) =>
        [RenderProjection(descriptor, context)];

    /// <summary>
    /// Renders the projection record with Chronicle model-bound attributes and embedded
    /// observable query methods.
    /// </summary>
    /// <param name="descriptor">The read model descriptor.</param>
    /// <param name="context">The code generation context.</param>
    static RenderedArtifact RenderProjection(ReadModelDescriptor descriptor, CodeGenerationContext context)
    {
        var conceptUsings = context.Concepts.ResolveConceptUsings(
            descriptor.Properties.Select(p => p.Type),
            context.Namespace);

        var builder = new CSharpCodeBuilder(context)
            .Using(
                "Cratis.Arc.MongoDB",
                "Cratis.Arc.Queries.ModelBound",
                "Cratis.Chronicle.Events",
                "Cratis.Chronicle.Keys",
                "Cratis.Chronicle.Projections.ModelBound",
                "Cratis.Chronicle.ReadModels",
                "MongoDB.Driver",
                "System.Reactive.Subjects")
            .Using([.. conceptUsings])
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
        }

        var pluralizedName = descriptor.Name.Pluralize();
        var keyProperty = properties.FirstOrDefault(p => p.IsKey);
        var keyType = keyProperty?.Type ?? "string";

        builder
            .Summary($"Gets all {descriptor.Name} instances as an observable collection.")
            .XmlParam("collection", $"The MongoDB collection for {descriptor.Name}.")
            .XmlReturns($"An observable subject of all {descriptor.Name} instances.")
            .ExpressionMember(
                $"ISubject<IEnumerable<{descriptor.Name}>>",
                $"All{pluralizedName}",
                "collection.Observe();",
                $"IMongoCollection<{descriptor.Name}> collection",
                isStatic: true)
            .BlankLine()
            .Summary($"Gets a single {descriptor.Name} by its identifier.")
            .XmlParam("collection", $"The MongoDB collection for {descriptor.Name}.")
            .XmlParam("id", "The identifier to look up.")
            .XmlReturns($"An observable subject of the matching {descriptor.Name}.")
            .ExpressionMember(
                $"ISubject<{descriptor.Name}>",
                $"{descriptor.Name}ById",
                "collection.ObserveById(id);",
                $"IMongoCollection<{descriptor.Name}> collection, {keyType} id",
                isStatic: true)
            .EndBlock();

        var artifactPath = Path.Combine(context.RelativePath, $"{descriptor.Name}.cs");

        return new RenderedArtifact(artifactPath, builder.ToString());
    }

    /// <summary>
    /// Appends record-level attributes.
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
