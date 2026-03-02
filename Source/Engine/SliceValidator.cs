// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.DependencyInjection;
using Cratis.VerticalSlices.EventModelAdvisory;

namespace Cratis.VerticalSlices;

/// <summary>
/// Validates a set of modules and their vertical slices before code generation begins.
/// Delegates all rule evaluation to <see cref="IEventModelAdvisor"/> and surfaces any
/// <see cref="EventModelRecommendationSeverity.Error"/>-level findings as a
/// <see cref="SliceValidationFailed"/> exception so callers see the full picture in one pass.
/// </summary>
[Singleton]
public class SliceValidator(IEventModelAdvisor advisor) : ISliceValidator
{
    /// <inheritdoc/>
    public void Validate(IEnumerable<Module> modules)
    {
        var moduleList = modules.ToList();
        var sliceTypesByName = BuildSliceTypeMap(moduleList);

        var errors = advisor.Analyze(moduleList)
            .Where(r => r.Severity == EventModelRecommendationSeverity.Error)
            .Select(r => new SliceValidationError(
                r.SliceName,
                sliceTypesByName.GetValueOrDefault(r.SliceName, VerticalSliceType.StateChange),
                r.Message))
            .ToList();

        if (errors.Count > 0)
        {
            throw new SliceValidationFailed(errors);
        }
    }

    static Dictionary<string, VerticalSliceType> BuildSliceTypeMap(IEnumerable<Module> modules)
    {
        var map = new Dictionary<string, VerticalSliceType>();
        foreach (var module in modules)
        {
            CollectSliceTypes(module.Features, map);
        }

        return map;
    }

    static void CollectSliceTypes(IEnumerable<Feature> features, Dictionary<string, VerticalSliceType> map)
    {
        foreach (var feature in features)
        {
            foreach (var slice in feature.VerticalSlices)
            {
                map[slice.Name] = slice.SliceType;
            }

            CollectSliceTypes(feature.Features, map);
        }
    }
}
