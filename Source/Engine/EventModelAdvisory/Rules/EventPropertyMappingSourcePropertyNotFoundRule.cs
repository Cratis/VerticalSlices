// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.EventModelAdvisory.Rules;

/// <summary>
/// Detects read model property mappings whose <see cref="EventPropertyMapping.SourcePropertyName"/>
/// does not match any property defined on the referenced event type.
/// When a source property is specified but cannot be resolved, code generation will produce
/// broken projection code — this must be treated as a hard error.
/// </summary>
/// <remarks>
/// Only mappings where <see cref="EventPropertyMapping.SourcePropertyName"/> is non-null are checked.
/// Mappings with an unresolvable <see cref="EventPropertyMapping.EventTypeName"/> are skipped here
/// and reported by <see cref="UnresolvedEventReferenceRule"/> instead.
/// </remarks>
public class EventPropertyMappingSourcePropertyNotFoundRule : IEventModelRule
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
                var eventsByName = slice.Events.ToDictionary(e => e.Name, StringComparer.OrdinalIgnoreCase);

                foreach (var readModel in slice.ReadModels)
                {
                    foreach (var property in readModel.Properties)
                    {
                        foreach (var mapping in property.Mappings.Where(m => !string.IsNullOrWhiteSpace(m.SourcePropertyName)))
                        {
                            if (!eventsByName.TryGetValue(mapping.EventTypeName, out var eventType))
                            {
                                continue; // Unresolved event type — handled by UnresolvedEventReferenceRule
                            }

                            var eventPropertyNames = new HashSet<string>(
                                eventType.Properties.Select(p => p.Name),
                                StringComparer.OrdinalIgnoreCase);

                            if (!eventPropertyNames.Contains(mapping.SourcePropertyName!))
                            {
                                yield return new EventModelRecommendation(
                                    EventModelRecommendationSeverity.Error,
                                    EventModelRecommendationCategory.Coverage,
                                    moduleName,
                                    featurePath,
                                    slice.Name,
                                    readModel.Name,
                                    $"Read model '{readModel.Name}' property '{property.Name}' maps from event '{mapping.EventTypeName}' using source property '{mapping.SourcePropertyName}', but that property does not exist on the event.",
                                    $"Add a property named '{mapping.SourcePropertyName}' to event '{mapping.EventTypeName}', or correct the source property name in the mapping.");
                            }
                        }
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
