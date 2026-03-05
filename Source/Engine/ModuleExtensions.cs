// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices;

/// <summary>
/// Extension methods for traversing the module and feature hierarchy.
/// </summary>
public static class ModuleExtensions
{
    /// <summary>
    /// Flattens the module and feature hierarchy into a depth-first sequence of
    /// (module name, feature path, slice) tuples. Eliminates the recursive
    /// traversal boilerplate that each rule would otherwise repeat.
    /// </summary>
    /// <param name="modules">The top-level modules to traverse.</param>
    /// <returns>A flat sequence of all slices with their location context.</returns>
    public static IEnumerable<(string ModuleName, FeaturePath Path, VerticalSlice Slice)> FlattenSlices(
        this IEnumerable<Module> modules)
    {
        foreach (var module in modules)
        {
            foreach (var item in module.Features.FlattenSlices(module.Name))
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Flattens a feature subtree into a depth-first sequence of (module name, feature path, slice) tuples.
    /// Useful when traversal must remain scoped to one module (e.g. for per-module cross-slice rules).
    /// </summary>
    /// <param name="features">The feature subtree to traverse.</param>
    /// <param name="moduleName">The name of the owning module.</param>
    /// <param name="rootPath">The feature path from which to start. Defaults to <see cref="FeaturePath.Empty"/>.</param>
    /// <returns>A flat sequence of all slices with their location context.</returns>
    public static IEnumerable<(string ModuleName, FeaturePath Path, VerticalSlice Slice)> FlattenSlices(
        this IEnumerable<Feature> features, string moduleName, FeaturePath? rootPath = null)
    {
        var path = rootPath ?? FeaturePath.Empty;
        foreach (var feature in features)
        {
            var featurePath = path.Append(feature.Name);
            foreach (var slice in feature.VerticalSlices)
            {
                yield return (moduleName, featurePath, slice);
            }

            foreach (var item in feature.Features.FlattenSlices(moduleName, featurePath))
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Flattens a feature subtree into a depth-first sequence of
    /// (module name, feature path, accumulated concept scope, feature) tuples.
    /// The concept scope in each tuple accumulates across the full ancestor chain
    /// (parent scope → this feature's concepts), so callers can build
    /// <see cref="CodeGeneration.CodeGenerationContext"/> without managing recursion.
    /// </summary>
    /// <param name="features">The feature subtree to traverse.</param>
    /// <param name="moduleName">The name of the owning module.</param>
    /// <param name="rootNamespace">The root namespace prefix used to derive per-feature namespaces. Matches <see cref="CodeGeneration.CodeGenerationOptions.RootNamespace"/>.</param>
    /// <param name="parentScope">The concept scope inherited from the parent level. Defaults to <see cref="ConceptScope.Empty"/>.</param>
    /// <param name="rootPath">The feature path offset from which to start. Defaults to <see cref="FeaturePath.Empty"/>.</param>
    /// <returns>A flat depth-first sequence of all features with their location and accumulated concept context.</returns>
    public static IEnumerable<(string ModuleName, FeaturePath Path, ConceptScope Scope, Feature Feature)> FlattenFeatures(
        this IEnumerable<Feature> features,
        string moduleName,
        string rootNamespace = "",
        ConceptScope? parentScope = null,
        FeaturePath? rootPath = null)
    {
        var inheritedScope = parentScope ?? ConceptScope.Empty;
        var path = rootPath ?? FeaturePath.Empty;

        foreach (var feature in features)
        {
            var featurePath = path.Append(feature.Name);
            var featureNamespace = BuildNamespace(rootNamespace, moduleName, featurePath);
            var featureScope = inheritedScope.With(feature.Concepts, featureNamespace);

            yield return (moduleName, featurePath, featureScope, feature);

            foreach (var item in feature.Features.FlattenFeatures(moduleName, rootNamespace, featureScope, featurePath))
            {
                yield return item;
            }
        }
    }

    static string BuildNamespace(string rootNamespace, string moduleName, FeaturePath featurePath)
    {
        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(rootNamespace)) parts.Add(rootNamespace);
        parts.Add(moduleName);
        parts.AddRange(featurePath.Segments.Where(s => !string.IsNullOrWhiteSpace(s)));
        return string.Join('.', parts);
    }
}
