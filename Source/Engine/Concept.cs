// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents a domain concept — a value type wrapping a single primitive with optional validation.
/// Concepts are scoped to a module, feature, or slice and act as reusable data types
/// for properties on commands, events, and read models.
/// </summary>
/// <param name="Name">The name of the concept (e.g., Email, FullName, OrderId).</param>
/// <param name="UnderlyingType">The primitive type the concept wraps (e.g., string, Guid, int).</param>
/// <param name="Description">A description of what the concept represents.</param>
/// <param name="ValidationRules">The validation rules that apply to this concept.</param>
/// <param name="IsEventSourceId">When <see langword="true"/>, the concept inherits from <c>EventSourceId</c> rather than <c>ConceptAs&lt;T&gt;</c>, acting as a typed event source identifier.</param>
public record Concept(
    string Name,
    string UnderlyingType,
    string Description,
    IEnumerable<ConceptValidationRule> ValidationRules,
    bool IsEventSourceId = false);
