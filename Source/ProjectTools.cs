// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
using ModelContextProtocol;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using static ModelContextProtocol.Protocol.ElicitRequestParams;

namespace Cratis.VerticalSlices;

/// <summary>
/// Provides a set of tools for working with the file system.
/// </summary>
[McpServerToolType]
public static class ProjectTools
{
    /// <summary>
    /// Sets the current project file in the vertical slices configuration.
    /// </summary>
    /// <param name="server">The MCP server.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The path to the active project file.</returns>
    /// <exception cref="McpException">Thrown when no project files are found or selection is cancelled.</exception>
    [McpServerTool, Description("Set the active project by selecting a .csproj file from the workspace.")]
    public static async Task<string> SetActiveProject(IMcpServer server, CancellationToken ct)
    {
        var roots = await server.RequestRootsAsync(new ListRootsRequestParams(), ct);
        var root = roots.Roots?[0]?.Uri ?? throw new McpException("No roots available from client.");
        var rootPath = new Uri(root).LocalPath;
        if (VerticalSlices.TryGetFrom(rootPath, out var configuration))
        {
            return configuration.ProjectFile;
        }

        var projectFiles = Directory.GetFiles(rootPath, "*.csproj", SearchOption.AllDirectories);
        if (projectFiles.Length == 0)
            throw new McpException("No project files found in workspace.");

        var relativeProjectFiles = projectFiles.Select(projectFile => Path.GetRelativePath(new Uri(root).LocalPath, projectFile)).ToArray();
        var schema = new RequestSchema
        {
            Properties =
            {
                ["project"] = new EnumSchema
                {
                    Title = "Select project",
                    Description = "Choose the project that holds the vertical slices.",
                    Enum = relativeProjectFiles,
                    EnumNames = relativeProjectFiles.Select(p => Path.GetFileName(p)).ToArray(),
                },
            },
            Required = ["project"]
        };

        var response = await server.ElicitAsync(new ElicitRequestParams
        {
            Message = "Select project file to use as active project",
            RequestedSchema = schema,
        });

        if (response.Action != "accept" || response.Content is null || !response.Content.TryGetValue("project", out var project))
            throw new McpException("Selection cancelled.");

        var projectFile = project.GetString() ?? throw new McpException("No project selected.");
        VerticalSlices.SetCurrentProject(rootPath, projectFile);
        return projectFile;
    }
}
