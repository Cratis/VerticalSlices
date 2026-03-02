// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.EventModelAdvisory.Rules;

/// <summary>
/// Detects <see cref="VerticalSliceType.StateView"/> slices that define no read models.
/// A StateView without read models has no projection target — incoming events project into
/// nothing and no data surface is exposed to the screen.
/// </summary>
public class StateViewWithNoReadModelsRule : IEventModelRule
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
            foreach (var slice in feature.VerticalSlices.Where(s => s.SliceType == VerticalSliceType.StateView && !s.ReadModels.Any()))
            {
                yield return new EventModelRecommendation(
                    EventModelRecommendationSeverity.Warning,
                    EventModelRecommendationCategory.Structure,
                    moduleName,
                    featurePath,
                    slice.Name,
                    slice.Name,
                    $"StateView slice '{slice.Name}' has no read models. A StateView projects events into read models displayed on a screen.",
                    "Add at least one read model to define what data this slice projects and exposes.");
            }

            foreach (var recommendation in EvaluateFeatures(feature.Features, moduleName, featurePath))
            {
                yield return recommendation;
            }
        }
    }
}
