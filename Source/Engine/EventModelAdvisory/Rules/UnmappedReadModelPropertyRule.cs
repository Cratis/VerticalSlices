// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.EventModelAdvisory.Rules;

/// <summary>
/// Detects read model properties that have no event-to-property mappings defined.
/// Properties without mappings will not be populated by the projection and may
/// indicate an incomplete model definition.
/// </summary>
public class UnmappedReadModelPropertyRule : IEventModelRule
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
            foreach (var slice in feature.VerticalSlices)
            {
                foreach (var readModel in slice.ReadModels)
                {
                    foreach (var property in readModel.Properties.Where(p => !p.Mappings.Any()))
                    {
                        yield return new EventModelRecommendation(
                            EventModelRecommendationSeverity.Information,
                            EventModelRecommendationCategory.Coverage,
                            moduleName,
                            featurePath,
                            slice.Name,
                            readModel.Name,
                            $"Read model property '{property.Name}' on '{readModel.Name}' has no event mappings.",
                            "Add an event-to-property mapping so the projection populates this property, or remove it if it is not needed.");
                    }
                }
            }

            foreach (var recommendation in EvaluateFeatures(feature.Features, moduleName, featurePath))
            {
                yield return recommendation;
            }
        }
    }
}
