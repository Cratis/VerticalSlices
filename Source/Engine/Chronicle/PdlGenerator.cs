// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;
using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.Chronicle;

/// <summary>
/// Generates Chronicle Projection Declaration Language (PDL) strings from read model descriptors.
/// PDL is used by Chronicle's SaveProjection REST API to register projections dynamically.
/// </summary>
public static class PdlGenerator
{
    /// <summary>
    /// Generates a PDL declaration from a read model descriptor.
    /// </summary>
    /// <param name="descriptor">The read model descriptor to convert to PDL.</param>
    /// <returns>A PDL declaration string.</returns>
    public static string Generate(ReadModelDescriptor descriptor)
    {
        var builder = new StringBuilder()
            .AppendLine($"projection {descriptor.Name}")
            .AppendLine("{");

        AppendKeyMapping(builder, descriptor);
        AppendFromBlocks(builder, descriptor);
        AppendEveryBlock(builder, descriptor);
        AppendRemoval(builder, descriptor);

        builder.AppendLine("}");

        return builder.ToString();
    }

    /// <summary>
    /// Appends the key property mapping if one exists.
    /// </summary>
    /// <param name="builder">The string builder.</param>
    /// <param name="descriptor">The read model descriptor.</param>
    static void AppendKeyMapping(StringBuilder builder, ReadModelDescriptor descriptor)
    {
        var keyProperty = descriptor.Properties.FirstOrDefault(p => p.IsKey);
        if (keyProperty is not null)
        {
            builder.AppendLine("    key = $eventSourceId");
        }
    }

    /// <summary>
    /// Appends "from EventType { ... }" blocks for each source event.
    /// </summary>
    /// <param name="builder">The string builder.</param>
    /// <param name="descriptor">The read model descriptor.</param>
    static void AppendFromBlocks(StringBuilder builder, ReadModelDescriptor descriptor)
    {
        var eventGroups = descriptor.Properties
            .SelectMany(p => p.Mappings.Select(m => new { Property = p, Mapping = m }))
            .Where(x => x.Mapping.EventTypeName != "*")
            .GroupBy(x => x.Mapping.EventTypeName);

        foreach (var group in eventGroups)
        {
            builder
                .AppendLine()
                .AppendLine($"    from {group.Key}")
                .AppendLine("    {");

            foreach (var item in group)
            {
                var expression = FormatMappingExpression(item.Mapping);
                if (expression is not null)
                {
                    builder.AppendLine($"        set {item.Property.Name} to {expression}");
                }
            }

            builder.AppendLine("    }");
        }
    }

    /// <summary>
    /// Appends the "from every" block if any every-event mappings exist.
    /// </summary>
    /// <param name="builder">The string builder.</param>
    /// <param name="descriptor">The read model descriptor.</param>
    static void AppendEveryBlock(StringBuilder builder, ReadModelDescriptor descriptor)
    {
        if (descriptor.EveryEventMappings?.Any() != true)
        {
            return;
        }

        builder
            .AppendLine()
            .AppendLine("    from every")
            .AppendLine("    {");

        foreach (var mapping in descriptor.EveryEventMappings)
        {
            builder.AppendLine($"        set {mapping.TargetProperty} to $context.{mapping.ContextProperty}");
        }

        builder.AppendLine("    }");
    }

    /// <summary>
    /// Appends the "removed with" directive if removal is configured.
    /// </summary>
    /// <param name="builder">The string builder.</param>
    /// <param name="descriptor">The read model descriptor.</param>
    static void AppendRemoval(StringBuilder builder, ReadModelDescriptor descriptor)
    {
        if (descriptor.Removal is null)
        {
            return;
        }

        builder
            .AppendLine()
            .AppendLine($"    removed with {descriptor.Removal.EventTypeName}");
    }

    /// <summary>
    /// Formats a property mapping into a PDL expression.
    /// </summary>
    /// <param name="mapping">The property mapping.</param>
    /// <returns>A PDL expression string, or null if the mapping kind is not supported in PDL.</returns>
    static string? FormatMappingExpression(PropertyMapping mapping) =>
        mapping.Kind switch
        {
            PropertyMappingKind.Set =>
                $"$event.{mapping.EventPropertyName}",

            PropertyMappingKind.Add =>
                $"$add($event.{mapping.EventPropertyName})",

            PropertyMappingKind.Subtract =>
                $"$subtract($event.{mapping.EventPropertyName})",

            PropertyMappingKind.Count =>
                "$count()",

            PropertyMappingKind.Increment =>
                "$increment()",

            PropertyMappingKind.Decrement =>
                "$decrement()",

            PropertyMappingKind.SetFromContext =>
                $"$context.{mapping.ContextProperty}",

            PropertyMappingKind.FromEventSourceId =>
                "$eventSourceId",

            _ => null
        };
}
