// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
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
    /// Reads a project file by path (relative to workspace).
    /// </summary>
    /// <param name="server">The MCP server.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The contents of the file.</returns>
    /// <exception cref="McpException">Thrown when the file is not found.</exception>
    [McpServerTool(ReadOnly = true), Description("Gets all the features in the application.")]
    public static async Task<IEnumerable<string>> GetFeatures(IMcpServer server, CancellationToken ct)
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
            return exportedTypes
                .Where(type => type.Namespace?.Split('.').Length > 1)
                .Select(type => type.Namespace!.Split('.')[1])
                .Distinct()
                .Order()
                .ToArray();
        }
        catch
        {
            throw new McpException($"Could not load assembly '{assemblyFile}'. Make sure the project has been built.");
        }
    }
}
