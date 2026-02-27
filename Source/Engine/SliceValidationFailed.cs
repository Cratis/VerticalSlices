// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices;

/// <summary>
/// The exception that is thrown when one or more vertical slices fail validation before code generation begins.
/// All violations are collected and reported together so callers see the full picture in one pass.
/// </summary>
/// <param name="errors">All validation errors that were found.</param>
public class SliceValidationFailed(IReadOnlyList<SliceValidationError> errors)
    : Exception(BuildMessage(errors))
{
    /// <summary>
    /// Gets all validation errors that caused this exception.
    /// </summary>
    public IReadOnlyList<SliceValidationError> Errors { get; } = errors;

    static string BuildMessage(IReadOnlyList<SliceValidationError> errors)
    {
        var lines = errors.Select(e => $"  [{e.SliceType}] '{e.SliceName}': {e.Message}");
        return $"{errors.Count} slice validation error(s) found:\n{string.Join('\n', lines)}";
    }
}
