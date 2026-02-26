// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration;

/// <summary>
/// Represents the context for code generation, carrying namespace hierarchy information.
/// </summary>
/// <param name="Module">The top-level module name.</param>
/// <param name="Feature">The feature name.</param>
/// <param name="SubFeatures">Any sub-feature names forming the hierarchy.</param>
public record CodeGenerationContext(string Module, string Feature, IEnumerable<string> SubFeatures)
{
    /// <summary>
    /// Gets the base namespace for generated code.
    /// </summary>
    public string Namespace
    {
        get
        {
            var parts = new List<string> { Module, Feature };
            parts.AddRange(SubFeatures);

            return string.Join('.', parts.Where(p => !string.IsNullOrWhiteSpace(p)));
        }
    }

    /// <summary>
    /// Gets the relative folder path matching the namespace hierarchy.
    /// </summary>
    public string RelativePath
    {
        get
        {
            var parts = new List<string> { Module, Feature };
            parts.AddRange(SubFeatures);

            return Path.Combine([.. parts.Where(p => !string.IsNullOrWhiteSpace(p))]);
        }
    }

    /// <summary>
    /// Creates a context with no hierarchy, using only the provided namespace directly.
    /// </summary>
    /// <param name="rootNamespace">The root namespace.</param>
    /// <returns>A new <see cref="CodeGenerationContext"/>.</returns>
    public static CodeGenerationContext FromNamespace(string rootNamespace) =>
        new(rootNamespace, string.Empty, []);
}
