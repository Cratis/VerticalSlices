// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices;

/// <summary>
/// Validates a set of modules and their vertical slices before code generation begins.
/// All violations are collected in a single pass and reported together via <see cref="SliceValidationFailed"/>.
/// </summary>
public static class SliceValidator
{
    /// <summary>
    /// Validates all slices reachable from the given modules.
    /// </summary>
    /// <param name="modules">The modules to validate.</param>
    /// <exception cref="SliceValidationFailed">Thrown when one or more validation errors are found.</exception>
    public static void Validate(IEnumerable<Module> modules)
    {
        var errors = new List<SliceValidationError>();

        foreach (var module in modules)
        {
            ValidateFeatures(module.Features, errors);
        }

        if (errors.Count > 0)
        {
            throw new SliceValidationFailed(errors);
        }
    }

    static void ValidateFeatures(IEnumerable<Feature> features, List<SliceValidationError> errors)
    {
        foreach (var feature in features)
        {
            foreach (var slice in feature.VerticalSlices)
            {
                ValidateSlice(slice, errors);
            }

            if (feature.Features.Any())
            {
                ValidateFeatures(feature.Features, errors);
            }
        }
    }

    static void ValidateSlice(VerticalSlice slice, List<SliceValidationError> errors)
    {
        if (slice.SliceType == VerticalSliceType.Translator)
        {
            return;
        }

        foreach (var externalEvent in slice.Events.Where(e => e.Kind == EventKind.External))
        {
            errors.Add(new SliceValidationError(
                slice.Name,
                slice.SliceType,
                $"Event '{externalEvent.Name}' is External. External events are only valid in Translator slices."));
        }
    }
}
