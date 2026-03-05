// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents a validation rule applied to a concept.
/// </summary>
/// <param name="Type">The kind of validation rule.</param>
/// <param name="Value">An optional parameter for the rule, such as a length limit or regex pattern.</param>
/// <param name="Message">An optional custom error message for the rule.</param>
public record ConceptValidationRule(ConceptValidationRuleType Type, string? Value = null, string? Message = null);
