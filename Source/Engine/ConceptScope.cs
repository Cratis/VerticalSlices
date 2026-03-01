// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents the set of concepts available at a given point in the module/feature hierarchy.
/// When traversing the hierarchy, each level can add its own concepts using <see cref="With"/>,
/// building an accumulated scope where inner concepts override outer ones with the same name.
/// </summary>
/// <param name="ConceptsByName">The concepts indexed by name (case-insensitive).</param>
public record ConceptScope(IReadOnlyDictionary<string, ScopedConcept> ConceptsByName)
{
    /// <summary>
    /// Gets an empty concept scope with no concepts in scope.
    /// </summary>
    public static ConceptScope Empty { get; } = new(new Dictionary<string, ScopedConcept>(StringComparer.OrdinalIgnoreCase));

    /// <summary>
    /// Returns whether the given type name matches a concept in scope.
    /// </summary>
    /// <param name="typeName">The type name to check.</param>
    /// <returns><see langword="true"/> if the type name matches a concept in scope.</returns>
    public bool IsConcept(string typeName) => ConceptsByName.ContainsKey(typeName);

    /// <summary>
    /// Returns whether the given type name matches a concept marked as an event source identifier.
    /// </summary>
    /// <param name="typeName">The type name to check.</param>
    /// <returns><see langword="true"/> if the type name is a concept with <see cref="Concept.IsEventSourceId"/> set.</returns>
    public bool IsEventSourceIdConcept(string typeName) =>
        ConceptsByName.TryGetValue(typeName, out var scoped) && scoped.Concept.IsEventSourceId;

    /// <summary>
    /// Creates a new scope by adding the given concepts at the specified namespace.
    /// Concepts with the same name as existing ones override them (inner scope wins).
    /// </summary>
    /// <param name="concepts">The concepts to add.</param>
    /// <param name="ns">The namespace where these concepts are rendered.</param>
    /// <returns>A new <see cref="ConceptScope"/> with the concepts added.</returns>
    public ConceptScope With(IEnumerable<Concept> concepts, string ns)
    {
        var dict = new Dictionary<string, ScopedConcept>(ConceptsByName, StringComparer.OrdinalIgnoreCase);

        foreach (var concept in concepts)
        {
            dict[concept.Name] = new ScopedConcept(concept, ns);
        }

        return new(dict);
    }

    /// <summary>
    /// Returns the distinct namespaces of all concepts referenced by the given type names
    /// that differ from the current namespace. Used by renderers to add <c>using</c> directives
    /// for concept types defined in other scopes.
    /// </summary>
    /// <param name="typeNames">The property type names to check.</param>
    /// <param name="currentNamespace">The namespace of the file being generated.</param>
    /// <returns>The concept namespaces that need <c>using</c> directives.</returns>
    public IEnumerable<string> ResolveConceptUsings(IEnumerable<string> typeNames, string currentNamespace) =>
        typeNames
            .Where(IsConcept)
            .Select(t => GetNamespace(t)!)
            .Where(ns => !ns.Equals(currentNamespace, StringComparison.OrdinalIgnoreCase))
            .Distinct();

    string? GetNamespace(string typeName) =>
        ConceptsByName.TryGetValue(typeName, out var scoped) ? scoped.Namespace : null;
}
