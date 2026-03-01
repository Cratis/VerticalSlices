// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents a concept together with the namespace where it is rendered.
/// Used during code generation to resolve property types that reference concepts
/// and add the appropriate <c>using</c> directives.
/// </summary>
/// <param name="Concept">The concept definition.</param>
/// <param name="Namespace">The namespace where this concept's generated code lives.</param>
public record ScopedConcept(Concept Concept, string Namespace);
