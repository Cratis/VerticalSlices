// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.Descriptors;

/// <summary>
/// Describes the structure of a concept for code generation.
/// </summary>
/// <param name="Name">The concept type name.</param>
/// <param name="UnderlyingType">The primitive type the concept wraps.</param>
/// <param name="Description">A description of what the concept represents.</param>
/// <param name="ValidationRules">The validation rules to generate.</param>
public record ConceptDescriptor(
    string Name,
    string UnderlyingType,
    string Description,
    IEnumerable<ConceptValidationRule> ValidationRules)
{
    /// <summary>
    /// Gets whether this concept has any validation rules defined.
    /// </summary>
    public bool HasValidation => ValidationRules.Any();

    /// <summary>
    /// Creates a ConceptDescriptor from a Concept domain model.
    /// </summary>
    /// <param name="concept">The concept to create a descriptor from.</param>
    /// <returns>A new <see cref="ConceptDescriptor"/>.</returns>
    public static ConceptDescriptor FromConcept(Concept concept) =>
        new(concept.Name, concept.UnderlyingType, concept.Description, concept.ValidationRules);
}
