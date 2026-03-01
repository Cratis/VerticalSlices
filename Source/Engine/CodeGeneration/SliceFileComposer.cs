// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;
using System.Text.RegularExpressions;

namespace Cratis.VerticalSlices.CodeGeneration;

/// <summary>
/// Composes multiple rendered artifacts for a single slice into one combined file.
/// Merges using directives, deduplicates namespaces, and concatenates the type declarations.
/// </summary>
public static partial class SliceFileComposer
{
    /// <summary>
    /// Combines multiple rendered artifacts into a single file under the given slice name.
    /// Using directives are deduplicated and sorted. Namespace declarations are unified.
    /// </summary>
    /// <param name="artifacts">The individual artifacts to combine.</param>
    /// <param name="context">The code generation context carrying namespace and path information.</param>
    /// <returns>A single <see cref="RenderedArtifact"/> containing all types.</returns>
    public static RenderedArtifact Compose(IEnumerable<RenderedArtifact> artifacts, CodeGenerationContext context)
    {
        var allUsings = new HashSet<string>();
        var typeDeclarations = new List<string>();

        foreach (var artifact in artifacts)
        {
            var (usings, body) = ParseArtifact(artifact.Content);
            foreach (var u in usings)
            {
                allUsings.Add(u);
            }

            if (!string.IsNullOrWhiteSpace(body))
            {
                typeDeclarations.Add(body.Trim());
            }
        }

        var result = new StringBuilder();

        if (allUsings.Count > 0)
        {
            foreach (var ns in allUsings.Order(StringComparer.Ordinal))
            {
                result.AppendLine($"using {ns};");
            }

            result.AppendLine();
        }

        result.AppendLine($"namespace {context.Namespace};");

        foreach (var declaration in typeDeclarations)
        {
            result
                .AppendLine()
                .AppendLine(declaration);
        }

        var fileName = string.IsNullOrWhiteSpace(context.SliceName)
            ? "Slice.cs"
            : $"{context.SliceName}.cs";

        var artifactPath = Path.Combine(context.RelativePath, fileName);

        return new RenderedArtifact(artifactPath, result.ToString());
    }

    static (HashSet<string> Usings, string Body) ParseArtifact(string content)
    {
        var usings = new HashSet<string>();
        var bodyLines = new List<string>();
        var pastUsings = false;

        foreach (var line in content.Split('\n'))
        {
            var trimmed = line.TrimEnd('\r');

            if (!pastUsings)
            {
                var usingMatch = UsingRegex().Match(trimmed);
                if (usingMatch.Success)
                {
                    usings.Add(usingMatch.Groups["ns"].Value);

                    continue;
                }

                if (string.IsNullOrWhiteSpace(trimmed))
                {
                    continue;
                }

                pastUsings = true;
            }

            if (NamespaceRegex().IsMatch(trimmed))
            {
                continue;
            }

            bodyLines.Add(trimmed);
        }

        // Remove leading/trailing blank lines from body
        while (bodyLines.Count > 0 && string.IsNullOrWhiteSpace(bodyLines[0]))
        {
            bodyLines.RemoveAt(0);
        }

        while (bodyLines.Count > 0 && string.IsNullOrWhiteSpace(bodyLines[^1]))
        {
            bodyLines.RemoveAt(bodyLines.Count - 1);
        }

        return (usings, string.Join('\n', bodyLines));
    }

    [GeneratedRegex(@"^using\s+(?<ns>[\w.]+)\s*;", RegexOptions.ExplicitCapture, matchTimeoutMilliseconds: 1000)]
    private static partial Regex UsingRegex();

    [GeneratedRegex(@"^namespace\s+[\w.]+\s*;", RegexOptions.ExplicitCapture, matchTimeoutMilliseconds: 1000)]
    private static partial Regex NamespaceRegex();
}
