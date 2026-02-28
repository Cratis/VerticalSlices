// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;
using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound;

/// <summary>
/// Renders a read model descriptor as a Chronicle model-bound projection using
/// [Key], [SetFrom], [AddFrom], [SubtractFrom], [Count], [Increment], [Decrement],
/// [SetFromContext], [FromEvery], [FromEvent], [ChildrenFrom], [Join], [RemovedWith],
/// [Passive], [NotRewindable], and [FromEventSequence] attributes.
/// Also generates an Observable query for the read model.
/// </summary>
public class ModelBoundReadModelRenderer : IArtifactRenderer<ReadModelDescriptor>
{
    /// <inheritdoc/>
    public IEnumerable<GeneratedFile> Render(ReadModelDescriptor descriptor, CodeGenerationContext context) =>
        [
            RenderProjection(descriptor, context),
            RenderObservableQuery(descriptor, context)
        ];

    /// <summary>
    /// Renders the projection record with Chronicle model-bound attributes.
    /// </summary>
    /// <param name="descriptor">The read model descriptor.</param>
    /// <param name="context">The code generation context.</param>
    /// <returns>A generated file for the projection.</returns>
    static GeneratedFile RenderProjection(ReadModelDescriptor descriptor, CodeGenerationContext context)
    {
        var usings = new List<string>
        {
            "Cratis.Chronicle.Events",
            "Cratis.Chronicle.Keys",
            "Cratis.Chronicle.Projections.ModelBound"
        };

        var builder = new StringBuilder()
            .AppendLine(CodeWriter.FormatUsings(usings))
            .AppendLine()
            .AppendLine($"namespace {context.Namespace};");

        builder.AppendLine();

        if (!string.IsNullOrWhiteSpace(descriptor.Description))
        {
            builder
                .AppendLine("/// <summary>")
                .AppendLine($"/// {descriptor.Description}")
                .AppendLine("/// </summary>");
        }

        AppendTypeAttributes(builder, descriptor);

        var properties = descriptor.Properties.ToList();
        builder.AppendLine($"public record {descriptor.Name}(");

        for (var i = 0; i < properties.Count; i++)
        {
            var property = properties[i];
            var suffix = i < properties.Count - 1 ? "," : ");";

            AppendPropertyAttributes(builder, property);
            builder.AppendLine($"    {property.Type} {property.Name}{suffix}");

            if (i < properties.Count - 1)
            {
                builder.AppendLine();
            }
        }

        var relativePath = Path.Combine(context.RelativePath, $"{descriptor.Name}.cs");

        return new GeneratedFile(relativePath, builder.ToString());
    }

    /// <summary>
    /// Renders an Observable query for the read model using the Arc [ReadModel] attribute
    /// with a static method returning ISubject&lt;IEnumerable&lt;T&gt;&gt;.
    /// </summary>
    /// <param name="descriptor">The read model descriptor.</param>
    /// <param name="context">The code generation context.</param>
    /// <returns>A generated file for the Observable query.</returns>
    static GeneratedFile RenderObservableQuery(ReadModelDescriptor descriptor, CodeGenerationContext context)
    {
        var queryName = $"All{descriptor.Name}s";

        var builder = new StringBuilder()
            .AppendLine(CodeWriter.FormatUsings(["Cratis.Arc.Queries.ModelBound"]))
            .AppendLine()
            .AppendLine($"namespace {context.Namespace};")
            .AppendLine()
            .AppendLine("/// <summary>")
            .AppendLine($"/// Observable query for all {descriptor.Name} instances.")
            .AppendLine("/// </summary>")
            .AppendLine("[ReadModel]")
            .AppendLine($"public partial record {queryName}")
            .AppendLine("{")
            .AppendLine("    /// <summary>")
            .AppendLine($"    /// Gets all {descriptor.Name} instances as an observable subject.")
            .AppendLine("    /// </summary>")
            .AppendLine("    /// <param name=\"collection\">The MongoDB collection.</param>")
            .AppendLine($"    /// <returns>An observable subject of all {descriptor.Name} instances.</returns>")
            .AppendLine($"    public static ISubject<IEnumerable<{descriptor.Name}>> GetAll(IMongoCollection<{descriptor.Name}> collection) =>")
            .AppendLine("        collection.Observe();")
            .AppendLine("}");

        var relativePath = Path.Combine(context.RelativePath, $"{queryName}.cs");

        return new GeneratedFile(relativePath, builder.ToString());
    }

    /// <summary>
    /// Appends record-level attributes such as [Passive], [NotRewindable], [FromEventSequence],
    /// [RemovedWith], and [FromEvent] for auto-mapped events.
    /// </summary>
    /// <param name="builder">The string builder.</param>
    /// <param name="descriptor">The read model descriptor.</param>
    static void AppendTypeAttributes(StringBuilder builder, ReadModelDescriptor descriptor)
    {
        if (descriptor.IsPassive)
        {
            builder.AppendLine("[Passive]");
        }

        if (descriptor.IsNotRewindable)
        {
            builder.AppendLine("[NotRewindable]");
        }

        if (!string.IsNullOrWhiteSpace(descriptor.EventSequence))
        {
            builder.AppendLine($"[FromEventSequence(\"{descriptor.EventSequence}\")]");
        }

        if (descriptor.Removal is not null)
        {
            builder.AppendLine($"[RemovedWith<{descriptor.Removal.EventTypeName}>]");
        }
    }

    /// <summary>
    /// Appends per-property attributes based on the property's mappings.
    /// </summary>
    /// <param name="builder">The string builder.</param>
    /// <param name="property">The property descriptor.</param>
    static void AppendPropertyAttributes(StringBuilder builder, ReadModelPropertyDescriptor property)
    {
        if (property.IsKey)
        {
            builder.AppendLine("    [Key]");
        }

        foreach (var mapping in property.Mappings)
        {
            var attribute = mapping.Kind switch
            {
                PropertyMappingKind.Set =>
                    $"[SetFrom<{mapping.EventTypeName}>(nameof({mapping.EventTypeName}.{mapping.EventPropertyName}))]",

                PropertyMappingKind.Add =>
                    $"[AddFrom<{mapping.EventTypeName}>(nameof({mapping.EventTypeName}.{mapping.EventPropertyName}))]",

                PropertyMappingKind.Subtract =>
                    $"[SubtractFrom<{mapping.EventTypeName}>(nameof({mapping.EventTypeName}.{mapping.EventPropertyName}))]",

                PropertyMappingKind.Count =>
                    $"[Count<{mapping.EventTypeName}>]",

                PropertyMappingKind.Increment =>
                    $"[Increment<{mapping.EventTypeName}>]",

                PropertyMappingKind.Decrement =>
                    $"[Decrement<{mapping.EventTypeName}>]",

                PropertyMappingKind.SetFromContext when mapping.EventTypeName != "*" =>
                    $"[SetFromContext<{mapping.EventTypeName}>(nameof(EventContext.{mapping.ContextProperty}))]",

                PropertyMappingKind.StaticValue =>
                    null,

                PropertyMappingKind.FromEventSourceId =>
                    null,

                _ => null
            };

            if (attribute is not null)
            {
                builder.AppendLine($"    {attribute}");
            }
        }

        var everyMappings = property.Mappings
            .Where(m => m.Kind is PropertyMappingKind.SetFromContext && m.EventTypeName == "*");

        foreach (var every in everyMappings)
        {
            builder.AppendLine($"    [FromEvery(contextProperty: nameof(EventContext.{every.ContextProperty}))]");
        }
    }
}
