// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.EventModelAdvisory.Rules;

/// <summary>
/// Detects <see cref="VerticalSliceType.StateChange"/> slices that have no events.
/// A StateChange slice must produce at least one event to record a side-effect in the
/// domain; without an event the command handler has no observable result and the slice
/// cannot be code-generated correctly.
/// </summary>
public class StateChangeWithNoEventsRule : IEventModelRule
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
            foreach (var slice in feature.VerticalSlices.Where(s => s.SliceType == VerticalSliceType.StateChange && !s.Events.Any()))
            {
                yield return new EventModelRecommendation(
                    EventModelRecommendationSeverity.Error,
                    EventModelRecommendationCategory.Structure,
                    moduleName,
                    featurePath,
                    slice.Name,
                    string.Empty,
                    $"StateChange slice '{slice.Name}' has no events. A StateChange slice must produce at least one event to record a domain side-effect.",
                    "Add at least one event type to the slice, or change the slice type if no domain event should be recorded.");
            }

            foreach (var recommendation in EvaluateFeatures(feature.Features, moduleName, featurePath))
            {
                yield return recommendation;
            }
        }
    }
}
