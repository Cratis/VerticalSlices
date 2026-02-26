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

            // Process each namespace in this feature
            foreach (var namespaceGroup in featureGroup)
            {
                var namespaceParts = namespaceGroup.Key.Split('.');

                if (namespaceParts.Length == 1)
                {
                    // This is a direct vertical slice in the feature
                    var verticalSlice = BuildVerticalSlice(namespaceParts[0], namespaceGroup.ToArray());
                    if (verticalSlice is not null)
                        verticalSlices.Add(verticalSlice);
                }
                else
                {
                    // This belongs to a sub-feature - for now, we'll treat it as a vertical slice
                    // In a more complex implementation, we could build nested features
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

        var verticalSliceType = DetermineVerticalSliceType(commands, readModels, events);

        return new VerticalSlice(name, verticalSliceType, null, commands, readModels, events);
    }

    static string DetermineVerticalSliceType(Command[] commands, ReadModel[] readModels, EventType[] events)
    {
        var hasCommands = commands.Length > 0;
        var hasReadModels = readModels.Length > 0;
        var hasEvents = events.Length > 0;

        // StateChange - holds one or more commands
        if (hasCommands && !hasReadModels)
            return VerticalSliceTypes.StateChange;

        // StateView - holds one or more read models without commands
        if (hasReadModels && !hasCommands)
            return VerticalSliceTypes.StateView;

        // Automation - holds a read model and a command
        if (hasCommands && hasReadModels)
            return VerticalSliceTypes.Automation;

        // Translator - holds events and commands
        if (hasEvents && hasCommands)
            return VerticalSliceTypes.Translator;

        // Default fallback based on what's present
        if (hasCommands)
            return VerticalSliceTypes.StateChange;
        if (hasReadModels)
            return VerticalSliceTypes.StateView;

        return "Unknown";
    }

    static Command[] GetCommands(Type[] types)
    {
        return types
            .Where(type => type.CustomAttributes.Any(attr => attr.AttributeType.Name == "CommandAttribute"))
            .Select(type => new Command(type.Name, ExtractXmlDocumentationSummary(type), GetProperties(type)))
            .ToArray();
    }

    static ReadModel[] GetReadModels(Type[] types, EventType[] events)
    {
        return types
            .Where(type => type.CustomAttributes.Any(attr => attr.AttributeType.Name == "ReadModelAttribute"))
            .Select(type =>
            {
                var properties = GetProperties(type);
                var relatedEvents = FindRelatedEvents(type, events);

                return new ReadModel(type.Name, ExtractXmlDocumentationSummary(type), properties, relatedEvents);
            })
            .ToArray();
    }

    /// <summary>
    /// Finds events related to a read model by inspecting projection attributes
    /// (e.g. SetFrom, AddFrom, Count, RemovedWith) or falling back to all events.
    /// </summary>
    /// <param name="readModelType">The read model type to inspect.</param>
    /// <param name="allEvents">All event types discovered in the slice.</param>
    /// <returns>The event types this read model projects from.</returns>
    static EventType[] FindRelatedEvents(Type readModelType, EventType[] allEvents)
    {
        var eventNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Inspect type-level attributes (e.g. [RemovedWith<T>])
        foreach (var attr in readModelType.CustomAttributes)
        {
            if (attr.AttributeType.IsGenericType && attr.AttributeType.GenericTypeArguments.Length > 0)
            {
                eventNames.Add(attr.AttributeType.GenericTypeArguments[0].Name);
            }
        }

        // Inspect property-level attributes (e.g. [SetFrom<T>], [AddFrom<T>], [Count<T>])
        foreach (var prop in readModelType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            foreach (var attr in prop.CustomAttributes)
            {
                if (attr.AttributeType.IsGenericType && attr.AttributeType.GenericTypeArguments.Length > 0)
                {
                    eventNames.Add(attr.AttributeType.GenericTypeArguments[0].Name);
                }
            }
        }

        // If we found event references via attributes, filter to those
        if (eventNames.Count > 0)
        {
            return allEvents
                .Where(e => eventNames.Contains(e.Name))
                .ToArray();
        }

        // Fallback: associate all events with this read model
        return allEvents;
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
