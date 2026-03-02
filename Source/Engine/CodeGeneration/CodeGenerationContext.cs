// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration;

/// <summary>
/// Represents the context for code generation, carrying namespace hierarchy information.
/// The hierarchy follows Module → Feature → SubFeatures → Slice.
/// </summary>
public record CodeGenerationContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CodeGenerationContext"/> record.
    /// </summary>
    /// <param name="moduleName">The top-level module name.</param>
    /// <param name="featurePath">The path through the feature hierarchy from module level down.</param>
    /// <param name="sliceName">The name of the vertical slice. Used for file naming but excluded from the folder/namespace path.</param>
    /// <param name="options">The code generation options that control how output is emitted. Defaults to a new instance with all default settings when <see langword="null"/>.</param>
    /// <param name="concepts">The concepts available at this scope level. When a property type matches a concept name, renderers add the appropriate <c>using</c> directive.</param>
    public CodeGenerationContext(string moduleName, FeaturePath featurePath, string sliceName, CodeGenerationOptions? options = null, ConceptScope? concepts = null)
    {
        ModuleName = moduleName;
        FeaturePath = featurePath;
        SliceName = sliceName;
        Options = options ?? new();
        Concepts = concepts ?? ConceptScope.Empty;
    }

    /// <summary>
    /// Gets the top-level module name.
    /// </summary>
    public string ModuleName { get; init; }

    /// <summary>
    /// Gets the path through the feature hierarchy from module level down.
    /// </summary>
    public FeaturePath FeaturePath { get; init; }

    /// <summary>
    /// Gets the name of the vertical slice. Used for file naming but excluded from the folder/namespace path.
    /// </summary>
    public string SliceName { get; init; }

    /// <summary>
    /// Gets the code generation options that control how output is emitted.
    /// </summary>
    public CodeGenerationOptions Options { get; init; }

    /// <summary>
    /// Gets the concepts available at this scope level.
    /// When a property type matches a concept name, renderers add the appropriate <c>using</c> directive.
    /// </summary>
    public ConceptScope Concepts { get; init; }

    /// <summary>
    /// Gets the base namespace for generated code.
    /// Follows Module.Feature.SubFeature hierarchy, excluding the slice name.
    /// When <see cref="CodeGenerationOptions.RootNamespace"/> is set, it is prepended.
    /// </summary>
    public string Namespace
    {
        get
        {
            var parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(Options.RootNamespace))
            {
                parts.Add(Options.RootNamespace);
            }

            parts.Add(ModuleName);
            parts.AddRange(FeaturePath.Segments.Where(p => !string.IsNullOrWhiteSpace(p)));

            return string.Join('.', parts.Where(p => !string.IsNullOrWhiteSpace(p)));
        }
    }

    /// <summary>
    /// Gets the relative folder path matching the namespace hierarchy.
    /// When <see cref="CodeGenerationOptions.SliceOwnFolder"/> is <see langword="true"/> and
    /// <see cref="SliceName"/> is set, the slice name is appended as a subfolder of the feature folder.
    /// Otherwise only Module/Feature/SubFeature segments are used.
    /// </summary>
    public string RelativePath
    {
        get
        {
            var parts = new List<string> { ModuleName };
            parts.AddRange(FeaturePath.Segments.Where(p => !string.IsNullOrWhiteSpace(p)));

            if (Options.SliceOwnFolder && !string.IsNullOrWhiteSpace(SliceName))
            {
                parts.Add(SliceName);
            }

            return Path.Combine([.. parts.Where(p => !string.IsNullOrWhiteSpace(p))]);
        }
    }

    /// <summary>
    /// Creates a context with no hierarchy, using only the provided namespace directly.
    /// </summary>
    /// <param name="rootNamespace">The root namespace.</param>
    /// <param name="options">The code generation options. Defaults to all default settings when <see langword="null"/>.</param>
    /// <returns>A new <see cref="CodeGenerationContext"/>.</returns>
    public static CodeGenerationContext FromNamespace(string rootNamespace, CodeGenerationOptions? options = null) =>
        new(rootNamespace, FeaturePath.Empty, string.Empty, options);
}
