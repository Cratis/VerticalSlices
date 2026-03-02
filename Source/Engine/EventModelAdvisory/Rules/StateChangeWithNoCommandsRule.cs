// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.EventModelAdvisory.Rules;

/// <summary>
/// Detects <see cref="VerticalSliceType.StateChange"/> slices that have no commands.
/// A StateChange slice exists to capture user intent; without at least one command there
/// is no way to express that intent, making the slice structurally incomplete and
/// impossible to generate correct code for.
/// </summary>
public class StateChangeWithNoCommandsRule : IEventModelRule
{
    /// <inheritdoc/>
    public IEnumerable<EventModelRecommendation> Evaluate(IEnumerable<Module> modules)
    {
        foreach (var module in modules)
        {
            foreach (var recommendation in EvaluateFeatures(module.Features, module.Name, FeaturePath.Empty))
            {
                yield return recommendation;
            }
        }
    }

    static IEnumerable<EventModelRecommendation> EvaluateFeatures(IEnumerable<Feature> features, string moduleName, FeaturePath path)
    {
        foreach (var feature in features)
        {
            var featurePath = path.Append(feature.Name);
            foreach (var slice in feature.VerticalSlices.Where(s => s.SliceType == VerticalSliceType.StateChange && !s.Commands.Any()))
            {
                yield return new EventModelRecommendation(
                    EventModelRecommendationSeverity.Error,
                    EventModelRecommendationCategory.Structure,
                    moduleName,
                    featurePath,
                    slice.Name,
                    string.Empty,
                    $"StateChange slice '{slice.Name}' has no commands. A StateChange slice must have at least one command to capture user intent.",
                    "Add at least one command to the slice, or change the slice type if no user intent needs to be captured.");
            }

            foreach (var recommendation in EvaluateFeatures(feature.Features, moduleName, featurePath))
            {
                yield return recommendation;
            }
        }
    }
}
