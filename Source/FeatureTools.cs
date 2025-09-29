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

            features.Add(new Feature(featureName, subFeatures, verticalSlices));
        }

        return features;
    }

    static VerticalSlice? BuildVerticalSlice(string name, Type[] types)
    {
        var commands = GetCommands(types);
        var queries = GetQueries(types);
        var readModels = GetReadModels(types);
        var events = GetEvents(types);

        // If no vertical slice artifacts found, return null
        if (commands.Length == 0 && queries.Length == 0 && readModels.Length == 0 && events.Length == 0)
            return null;

        var verticalSliceType = DetermineVerticalSliceType(commands, queries, readModels, events);

        return new VerticalSlice(name, verticalSliceType, commands, queries, readModels, events);
    }

    static string DetermineVerticalSliceType(Command[] commands, Query[] queries, ReadModel[] readModels, EventType[] events)
    {
        var hasCommands = commands.Length > 0;
        var hasQueries = queries.Length > 0;
        var hasReadModels = readModels.Length > 0;
        var hasEvents = events.Length > 0;

        // StateChange - holds one or more commands
        if (hasCommands && !hasQueries && !hasReadModels)
            return VerticalSliceTypes.StateChange;

        // StateView - holds one or more queries
        if (hasQueries && hasReadModels && !hasCommands)
            return VerticalSliceTypes.StateView;

        // Automation - holds a read-model and a command
        if (hasCommands && hasReadModels && !hasQueries)
            return VerticalSliceTypes.Automation;

        // Translator - holds an event, a reactor and translates into a command
        if (hasEvents && hasCommands)
            return VerticalSliceTypes.Translator;

        // Default fallback based on what's present
        if (hasCommands)
            return VerticalSliceTypes.StateChange;
        if (hasQueries)
            return VerticalSliceTypes.StateView;

        return "Unknown";
    }

    static Command[] GetCommands(Type[] types)
    {
        return types
            .Where(type => type.CustomAttributes.Any(attr => attr.AttributeType.Name == "CommandAttribute"))
            .Select(type => new Command(type.Name, GetProperties(type)))
            .ToArray();
    }

    static ReadModel[] GetReadModels(Type[] types)
    {
        return types
            .Where(type => type.CustomAttributes.Any(attr => attr.AttributeType.Name == "ReadModelAttribute"))
            .Select(type => new ReadModel(type.Name, GetProperties(type)))
            .ToArray();
    }

    static EventType[] GetEvents(Type[] types)
    {
        return types
            .Where(type => type.CustomAttributes.Any(attr => attr.AttributeType.Name == "EventTypeAttribute"))
            .Select(type => new EventType(type.Name, GetProperties(type)))
            .ToArray();
    }

    static Query[] GetQueries(Type[] types)
    {
        var queries = new List<Query>();

        foreach (var readModelType in types.Where(type => type.CustomAttributes.Any(attr => attr.AttributeType.Name == "ReadModelAttribute")))
        {
            foreach (var method in readModelType.GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                var returnType = method.ReturnType;
                var readModelName = readModelType.Name;

                // Check if method returns the read model, a collection of it, or an ISubject<> of it
                if (IsQueryMethod(returnType, readModelType))
                {
                    var parameters = method.GetParameters()
                        .Select(p => new Property(p.Name ?? "parameter", GetTypeName(p.ParameterType)))
                        .ToArray();

                    queries.Add(new Query(method.Name, readModelName, parameters));
                }
            }
        }

        return [.. queries];
    }

    static bool IsQueryMethod(Type returnType, Type readModelType)
    {
        // Returns the read model directly
        if (returnType == readModelType)
            return true;

        // Returns a collection of the read model
        if (returnType.IsGenericType)
        {
            var genericType = returnType.GetGenericTypeDefinition();
            var genericArgs = returnType.GetGenericArguments();

            // Check for IEnumerable<ReadModel>, ICollection<ReadModel>, List<ReadModel>, etc.
            if (genericArgs.Length == 1 && genericArgs[0] == readModelType)
            {
                if (genericType.Name.StartsWith("IEnumerable") ||
                    genericType.Name.StartsWith("ICollection") ||
                    genericType.Name.StartsWith("List") ||
                    genericType.Name.StartsWith("ISubject"))
                {
                    return true;
                }
            }
        }

        return false;
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
}
