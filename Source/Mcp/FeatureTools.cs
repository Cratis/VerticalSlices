// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Reflection;
using ModelContextProtocol;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace Cratis.VerticalSlices;

/// <summary>
/// Provides a set of tools for working with the file system.
/// </summary>
[McpServerToolType]
public static class FeatureTools
{
    /// <summary>
    /// Gets all the features in the application with detailed information about vertical slices.
    /// </summary>
    /// <param name="server">The MCP server.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The features with all their details.</returns>
    /// <exception cref="McpException">Thrown when the assembly cannot be loaded.</exception>
    [McpServerTool(ReadOnly = true), Description("Gets all the features in the application.")]
    public static async Task<IEnumerable<Feature>> GetFeatures(IMcpServer server, CancellationToken ct)
    {
        var roots = await server.RequestRootsAsync(new ListRootsRequestParams(), ct);
        var root = roots.Roots?[0]?.Uri ?? throw new McpException("No roots available from client.");
        var rootPath = new Uri(root).LocalPath;
        var projectFile = string.Empty;
        if (VerticalSlices.TryGetFrom(rootPath, out var configuration))
        {
            projectFile = configuration.ProjectFile;
        }
        else
        {
            projectFile = await ProjectTools.SetActiveProject(server, ct);
        }

        var fullPath = Path.Combine(rootPath, projectFile);
        var projectPath = Path.GetDirectoryName(fullPath);
        if (projectPath is null || !Directory.Exists(projectPath))
            throw new McpException($"Project path '{projectPath}' not found.");

        var assemblyFile = Path.Combine(projectPath, "bin", "Debug", "net9.0", Path.GetFileNameWithoutExtension(fullPath) + ".dll");
        try
        {
            using var enumerator = new AssemblyTypeEnumerator(assemblyFile);
            var exportedTypes = enumerator.ExportedTypes;

            // Group types by namespace hierarchy
            var namespaceGroups = exportedTypes
                .Where(type => type.Namespace?.Split('.').Length > 1)
                .GroupBy(type => string.Join('.', type.Namespace!.Split('.').Skip(1)))
                .ToArray();

            return BuildFeatureHierarchy(namespaceGroups);
        }
        catch
        {
            throw new McpException($"Could not load assembly '{assemblyFile}'. Make sure the project has been built.");
        }
    }

    static List<Feature> BuildFeatureHierarchy(IGrouping<string, Type>[] namespaceGroups)
    {
        var features = new List<Feature>();

        // Group by first namespace segment (feature name)
        var featureGroups = namespaceGroups
            .GroupBy(group => group.Key.Split('.')[0])
            .ToArray();

        foreach (var featureGroup in featureGroups)
        {
            var featureName = featureGroup.Key;
            var subFeatures = new List<Feature>();
            var verticalSlices = new List<VerticalSlice>();
            var allTypesInFeature = featureGroup.SelectMany(g => g).ToArray();
            var concepts = GetConcepts(allTypesInFeature);

            foreach (var namespaceGroup in featureGroup)
            {
                var namespaceParts = namespaceGroup.Key.Split('.');

                if (namespaceParts.Length == 1)
                {
                    var verticalSlice = BuildVerticalSlice(namespaceParts[0], namespaceGroup.ToArray());
                    if (verticalSlice is not null)
                        verticalSlices.Add(verticalSlice);
                }
                else
                {
                    var verticalSlice = BuildVerticalSlice(namespaceParts[^1], namespaceGroup.ToArray());
                    if (verticalSlice is not null)
                        verticalSlices.Add(verticalSlice);
                }
            }

            features.Add(new Feature(featureName, concepts, subFeatures, verticalSlices));
        }

        return features;
    }

    static VerticalSlice? BuildVerticalSlice(string name, Type[] types)
    {
        var commands = GetCommands(types);
        var events = GetEvents(types);
        var readModels = GetReadModels(types, events);

        // If no vertical slice artifacts found, return null
        if (commands.Length == 0 && readModels.Length == 0 && events.Length == 0)
            return null;

        var sliceType = DetermineVerticalSliceType(commands, readModels, events);

        return new VerticalSlice(name, sliceType, null, null, commands, readModels, events);
    }

    static VerticalSliceType DetermineVerticalSliceType(Command[] commands, ReadModel[] readModels, EventType[] events)
    {
        var hasCommands = commands.Length > 0;
        var hasReadModels = readModels.Length > 0;
        var hasEvents = events.Length > 0;

        // StateChange - holds one or more commands
        if (hasCommands && !hasReadModels)
            return VerticalSliceType.StateChange;

        // StateView - holds one or more read models without commands
        if (hasReadModels && !hasCommands)
            return VerticalSliceType.StateView;

        // Automation - holds a read model and a command
        if (hasCommands && hasReadModels)
            return VerticalSliceType.Automation;

        // Translator - holds events and commands
        if (hasEvents && hasCommands)
            return VerticalSliceType.Translator;

        // Default fallback based on what's present
        if (hasCommands)
            return VerticalSliceType.StateChange;
        if (hasReadModels)
            return VerticalSliceType.StateView;

        return VerticalSliceType.StateChange;
    }

    static Command[] GetCommands(Type[] types)
    {
        return types
            .Where(type => type.CustomAttributes.Any(attr => attr.AttributeType.Name == "CommandAttribute"))
            .Select(type => new Command(type.Name, ExtractXmlDocumentationSummary(type), GetProperties(type)))
            .ToArray();
    }

    static ReadModel[] GetReadModels(Type[] types, EventType[] availableEvents)
    {
        return types
            .Where(type => type.CustomAttributes.Any(attr => attr.AttributeType.Name == "ReadModelAttribute"))
            .Select(type =>
            {
                var properties = GetReadModelProperties(type, availableEvents);
                return new ReadModel(type.Name, ExtractXmlDocumentationSummary(type), properties);
            })
            .ToArray();
    }

    /// <summary>
    /// Builds the read model property list for a given type by inspecting projection attributes
    /// (e.g. SetFrom, AddFrom, SubtractFrom, Count, Increment) on each property.
    /// Falls back to name-match Set mappings when no attributes are found.
    /// </summary>
    /// <param name="readModelType">The read model type to inspect.</param>
    /// <param name="availableEvents">All event types discovered in the slice.</param>
    static ReadModelProperty[] GetReadModelProperties(Type readModelType, EventType[] availableEvents)
    {
        return readModelType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(prop => prop.CanRead)
            .Select(prop =>
            {
                var mappings = ExtractPropertyMappings(prop, availableEvents);
                return new ReadModelProperty(prop.Name, GetTypeName(prop.PropertyType), mappings);
            })
            .ToArray();
    }

    /// <summary>
    /// Extracts explicit event-to-property mappings from Chronicle projection attributes
    /// on the given property. Falls back to name-based Set mappings across all available events
    /// when no attributes are found.
    /// </summary>
    /// <param name="prop">The property to inspect.</param>
    /// <param name="availableEvents">All event types discovered in the slice.</param>
    static EventPropertyMapping[] ExtractPropertyMappings(PropertyInfo prop, EventType[] availableEvents)
    {
        var mappings = new List<EventPropertyMapping>();

        foreach (var attr in prop.CustomAttributes)
        {
            if (!attr.AttributeType.IsGenericType || attr.AttributeType.GenericTypeArguments.Length == 0)
                continue;

            var eventTypeName = attr.AttributeType.GenericTypeArguments[0].Name;
            var attrName = attr.AttributeType.Name;

            var kind = attrName switch
            {
                var n when n.StartsWith("AddFrom", StringComparison.Ordinal) => EventPropertyMappingKind.Add,
                var n when n.StartsWith("SubtractFrom", StringComparison.Ordinal) => EventPropertyMappingKind.Subtract,
                var n when n.StartsWith("Count", StringComparison.Ordinal) => EventPropertyMappingKind.Count,
                var n when n.StartsWith("Increment", StringComparison.Ordinal) => EventPropertyMappingKind.Increment,
                var n when n.StartsWith("FromEventSourceId", StringComparison.Ordinal) => EventPropertyMappingKind.FromEventSourceId,
                _ => EventPropertyMappingKind.Set
            };

            // Count/Increment/FromEventSourceId do not read from a named event property
            var sourcePropertyName = kind is EventPropertyMappingKind.Set
                or EventPropertyMappingKind.Add
                or EventPropertyMappingKind.Subtract
                ? prop.Name
                : null;

            mappings.Add(new EventPropertyMapping(eventTypeName, kind, sourcePropertyName));
        }

        // Fallback: name-match Set mappings across all available events
        if (mappings.Count == 0)
        {
            foreach (var evt in availableEvents)
            {
                var match = evt.Properties
                    .FirstOrDefault(ep => ep.Name.Equals(prop.Name, StringComparison.OrdinalIgnoreCase));

                if (match is not null)
                    mappings.Add(new EventPropertyMapping(evt.Name, EventPropertyMappingKind.Set, match.Name));
            }
        }

        return [.. mappings];
    }

    static EventType[] GetEvents(Type[] types)
    {
        return types
            .Where(type => type.CustomAttributes.Any(attr => attr.AttributeType.Name == "EventTypeAttribute"))
            .Select(type => new EventType(type.Name, ExtractXmlDocumentationSummary(type), GetProperties(type)))
            .ToArray();
    }

    static Property[] GetProperties(Type type)
    {
        return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(prop => prop.CanRead)
            .Select(prop => new Property(prop.Name, GetTypeName(prop.PropertyType)))
            .ToArray();
    }

    static string GetTypeName(Type type)
    {
        if (type.IsGenericType)
        {
            var genericTypeName = type.Name.Split('`')[0];
            var genericArgs = string.Join(", ", type.GetGenericArguments().Select(GetTypeName));
            return $"{genericTypeName}<{genericArgs}>";
        }

        return type.Name switch
        {
            "String" => "string",
            "Int32" => "int",
            "Int64" => "long",
            "Boolean" => "bool",
            "Decimal" => "decimal",
            "Double" => "double",
            "Single" => "float",
            "Guid" => "Guid",
            "DateTime" => "DateTime",
            _ => type.Name
        };
    }

    static string ExtractXmlDocumentationSummary(Type type)
    {
        try
        {
            // Try to get XML documentation from assembly XML file
            var assemblyLocation = type.Assembly.Location;
            var xmlDocPath = Path.ChangeExtension(assemblyLocation, ".xml");

            if (File.Exists(xmlDocPath))
            {
                var xmlDoc = new System.Xml.XmlDocument();
                xmlDoc.Load(xmlDocPath);

                var typeName = type.FullName?.Replace('+', '.');
                var memberPath = $"T:{typeName}";

                var memberNode = xmlDoc.SelectSingleNode($"//member[@name='{memberPath}']");
                var summaryNode = memberNode?.SelectSingleNode("summary");

                if (summaryNode is not null)
                {
                    return summaryNode.InnerText.Trim();
                }
            }
        }
        catch
        {
            // If XML documentation extraction fails, fall back to empty string
        }

        return string.Empty;
    }

    /// <summary>
    /// Discovers concept types that derive from ConceptAs in the given types.
    /// </summary>
    /// <param name="types">The types to inspect for concept definitions.</param>
    static Concept[] GetConcepts(Type[] types) =>
        types
            .Where(IsConceptType)
            .Select(type =>
            {
                var underlyingType = GetConceptUnderlyingType(type);
                return new Concept(
                    type.Name,
                    GetTypeName(underlyingType),
                    ExtractXmlDocumentationSummary(type),
                    []);
            })
            .ToArray();

    /// <summary>
    /// Checks whether a type derives from ConceptAs.
    /// </summary>
    /// <param name="type">The type to check.</param>
    static bool IsConceptType(Type type)
    {
        var baseType = type.BaseType;
        while (baseType is not null)
        {
            if (baseType.IsGenericType && baseType.GetGenericTypeDefinition().Name.StartsWith("ConceptAs"))
            {
                return true;
            }

            baseType = baseType.BaseType;
        }

        return false;
    }

    /// <summary>
    /// Gets the underlying primitive type from a ConceptAs-derived type.
    /// </summary>
    /// <param name="type">The concept type to extract the underlying type from.</param>
    static Type GetConceptUnderlyingType(Type type)
    {
        var baseType = type.BaseType;
        while (baseType is not null)
        {
            if (baseType.IsGenericType && baseType.GetGenericTypeDefinition().Name.StartsWith("ConceptAs"))
            {
                return baseType.GetGenericArguments()[0];
            }

            baseType = baseType.BaseType;
        }

        return typeof(string);
    }
}
