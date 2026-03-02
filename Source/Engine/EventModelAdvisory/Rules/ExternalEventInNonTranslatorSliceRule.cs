// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.EventModelAdvisory.Rules;

/// <summary>
/// Detects external events used in slices other than <see cref="VerticalSliceType.Translator"/>.
/// External events represent domain events from outside the bounded context and can only be
/// consumed in Translator slices, which are specifically designed to bridge external event
/// streams into internal domain events. Using an external event in any other slice type
/// will lead to incorrect code generation.
/// </summary>
public class ExternalEventInNonTranslatorSliceRule : IEventModelRule
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
            foreach (var slice in feature.VerticalSlices.Where(s => s.SliceType != VerticalSliceType.Translator))
            {
                foreach (var externalEvent in slice.Events.Where(e => e.Kind == EventKind.External))
                {
                    yield return new EventModelRecommendation(
                        EventModelRecommendationSeverity.Error,
                        EventModelRecommendationCategory.Structure,
                        moduleName,
                        featurePath,
                        slice.Name,
                        externalEvent.Name,
                        $"Event '{externalEvent.Name}' is External and cannot be used in a {slice.SliceType} slice. External events are only valid in Translator slices.",
                        "Move the external event to a Translator slice, or change the event kind to Internal if it originates within this bounded context.");
                }
            }

            foreach (var recommendation in EvaluateFeatures(feature.Features, moduleName, featurePath))
            {
                yield return recommendation;
            }
        }
    }
}
