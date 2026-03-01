// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;
using Humanizer;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.Controller;

/// <summary>
/// Renders a read model descriptor as a projection record plus a separate ASP.NET Core controller
/// that provides query endpoints using <c>IMongoCollection&lt;T&gt;</c>.
/// The projection uses Chronicle model-bound attributes while the endpoint is controller-based.
/// </summary>
public class ControllerReadModelRenderer : IArtifactRenderer<ReadModelDescriptor>
{
    /// <inheritdoc/>
    public IEnumerable<RenderedArtifact> Render(ReadModelDescriptor descriptor, CodeGenerationContext context) =>
        [RenderProjection(descriptor, context), RenderController(descriptor, context)];

    /// <summary>
    /// Renders the read model record with projection attributes but without embedded query methods.
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
                "Cratis.Chronicle.Events",
                "Cratis.Chronicle.Keys",
                "Cratis.Chronicle.Projections.ModelBound",
                "Cratis.Chronicle.ReadModels")
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

        if (properties.Count == 0)
        {
            builder.Record(descriptor.Name);
        }
        else
        {
            builder.BeginPrimaryConstructorParameters(descriptor.Name);

            for (var i = 0; i < properties.Count; i++)
            {
                var property = properties[i];
                var isLast = i == properties.Count - 1;

                AppendPropertyAttributes(builder, property);
                builder.ConstructorParameter($"{property.Type} {property.Name}", isLast);
            }
        }

        var artifactPath = Path.Combine(context.RelativePath, $"{descriptor.Name}.cs");

        return new RenderedArtifact(artifactPath, builder.ToString());
    }

    /// <summary>
    /// Renders a controller with GET endpoints for all and by-id queries.
    /// </summary>
    /// <param name="descriptor">The read model descriptor.</param>
    /// <param name="context">The code generation context.</param>
    static RenderedArtifact RenderController(ReadModelDescriptor descriptor, CodeGenerationContext context)
    {
        var pluralizedName = descriptor.Name.Pluralize();
        var properties = descriptor.Properties.ToList();
        var keyProperty = properties.FirstOrDefault(p => p.IsKey);
        var keyType = keyProperty?.Type ?? "string";

        var builder = new CSharpCodeBuilder(context)
            .Using("Microsoft.AspNetCore.Mvc", "MongoDB.Driver", "System.Reactive.Subjects")
            .Namespace(context.Namespace)
            .BlankLine()
            .Summary($"Controller for querying <see cref=\"{descriptor.Name}\"/> read models.")
            .Attribute("ApiController")
            .Attribute($"Route(\"api/{pluralizedName.ToLowerInvariant()}\")");

        builder.OpenClass($"{descriptor.Name}Controller", "ControllerBase");

        // GET all
        builder
            .Summary($"Gets all {descriptor.Name} instances.")
            .XmlParam("collection", $"The MongoDB collection for {descriptor.Name}.")
            .XmlReturns($"All {descriptor.Name} instances.")
            .Attribute("HttpGet")
            .ExpressionMember(
                $"async Task<IEnumerable<{descriptor.Name}>>",
                $"GetAll{pluralizedName}",
                $"await collection.Find(FilterDefinition<{descriptor.Name}>.Empty).ToListAsync();",
                $"[FromServices] IMongoCollection<{descriptor.Name}> collection")
            .BlankLine();

        // GET by id
        builder
            .Summary($"Gets a single {descriptor.Name} by its identifier.")
            .XmlParam("collection", $"The MongoDB collection for {descriptor.Name}.")
            .XmlParam("id", "The identifier to look up.")
            .XmlReturns($"The matching {descriptor.Name}, or 404 if not found.")
            .Attribute("HttpGet(\"{id}\")");

        builder.OpenMethod(
            $"async Task<ActionResult<{descriptor.Name}>>",
            $"Get{descriptor.Name}ById",
            $"[FromServices] IMongoCollection<{descriptor.Name}> collection, [FromRoute] {keyType} id");

        builder
            .Statement($"var result = await collection.Find(Builders<{descriptor.Name}>.Filter.Eq(\"_id\", id)).FirstOrDefaultAsync();")
            .Statement("if (result is null)")
            .Statement("{")
            .Statement("    return NotFound();")
            .Statement("}")
            .BlankLine()
            .Statement("return Ok(result);")
            .EndBlock();

        builder.EndBlock();

        var artifactPath = Path.Combine(context.RelativePath, $"{descriptor.Name}Controller.cs");

        return new RenderedArtifact(artifactPath, builder.ToString());
    }

    /// <summary>
    /// Appends record-level attributes (same logic as model-bound renderer).
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
    /// Appends per-property attributes based on the property's mappings (same logic as model-bound renderer).
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
