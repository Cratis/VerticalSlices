// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices;

/// <summary>
/// Defines the types of validation rules that can be applied to a concept.
/// </summary>
public enum ConceptValidationRuleType
{
    /// <summary>
    /// The value must not be empty or default.
    /// </summary>
    NotEmpty = 0,

    /// <summary>
    /// The string value must have a minimum length.
    /// </summary>
    MinimumLength = 1,

    /// <summary>
    /// The string value must not exceed a maximum length.
    /// </summary>
    MaximumLength = 2,

    /// <summary>
    /// The string value must be a valid email address.
    /// </summary>
    EmailAddress = 3,

    /// <summary>
    /// The string value must match a regular expression pattern.
    /// </summary>
    Pattern = 4,

    /// <summary>
    /// The numeric value must be greater than a specified threshold.
    /// </summary>
    GreaterThan = 5,

    /// <summary>
    /// The numeric value must be less than a specified threshold.
    /// </summary>
    LessThan = 6,

    /// <summary>
    /// The numeric value must be greater than or equal to a specified threshold.
    /// </summary>
    GreaterThanOrEqualTo = 7,

    /// <summary>
    /// The numeric value must be less than or equal to a specified threshold.
    /// </summary>
    LessThanOrEqualTo = 8
}
