// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.EventModelAdvisory.Rules;

/// <summary>
/// Detects commands placed inside a <see cref="VerticalSliceType.StateView"/> slice.
/// StateView slices project events into read models displayed on a screen; issuing commands
/// from a StateView is an architectural violation and will result in incorrect code generation.
/// </summary>
public class StateViewWithCommandsRule : IEventModelRule
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
            foreach (var slice in feature.VerticalSlices.Where(s => s.SliceType == VerticalSliceType.StateView))
            {
                foreach (var command in slice.Commands)
                {
                    yield return new EventModelRecommendation(
                        EventModelRecommendationSeverity.Error,
                        EventModelRecommendationCategory.Structure,
                        moduleName,
                        featurePath,
                        slice.Name,
                        command.Name,
                        $"StateView slice '{slice.Name}' contains command '{command.Name}'. StateView slices project events into read models and do not issue commands.",
                        "Move the command to a StateChange or Automation slice, or change the slice type if the intent is to capture user intent.");
                }
            }

            foreach (var recommendation in EvaluateFeatures(feature.Features, moduleName, featurePath))
            {
                yield return recommendation;
            }
        }
    }
}
