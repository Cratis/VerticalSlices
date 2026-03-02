// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.EventModelAdvisory.Rules;

/// <summary>
/// Detects events that are defined in a module but never referenced by any
/// read model property mapping. These orphaned events produce data that
/// no projection consumes locally.
/// </summary>
public class OrphanedEventRule : IEventModelRule
{
    /// <inheritdoc/>
    public IEnumerable<EventModelRecommendation> Evaluate(IEnumerable<Module> modules)
    {
        var referencedEvents = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var definedEvents = new List<(string ModuleName, FeaturePath FeaturePath, string SliceName, string EventName)>();

        foreach (var module in modules)
        {
            CollectFromFeatures(module.Features, definedEvents, referencedEvents, module.Name, FeaturePath.Empty);
        }

        foreach (var (moduleName, featurePath, sliceName, eventName) in definedEvents.Where(e => !referencedEvents.Contains(e.EventName)))
        {
            yield return new EventModelRecommendation(
                EventModelRecommendationSeverity.Suggestion,
                EventModelRecommendationCategory.Coverage,
                moduleName,
                featurePath,
                sliceName,
                eventName,
                $"Event type '{eventName}' is not referenced by any read model property mapping.",
                "If this event is consumed by an external system or a different module, consider documenting that relationship. Otherwise, add a read model that projects this event.");
        }
    }

    static void CollectFromFeatures(
        IEnumerable<Feature> features,
        List<(string ModuleName, FeaturePath FeaturePath, string SliceName, string EventName)> definedEvents,
        HashSet<string> referencedEvents,
        string moduleName,
        FeaturePath path)
    {
        foreach (var feature in features)
        {
            var featurePath = path.Append(feature.Name);
            foreach (var slice in feature.VerticalSlices)
            {
                foreach (var eventType in slice.Events)
                {
                    definedEvents.Add((moduleName, featurePath, slice.Name, eventType.Name));
                }

                foreach (var readModel in slice.ReadModels)
                {
                    foreach (var mapping in readModel.Properties.SelectMany(p => p.Mappings))
                    {
                        referencedEvents.Add(mapping.EventTypeName);
                    }
                }
            }

            CollectFromFeatures(feature.Features, definedEvents, referencedEvents, moduleName, featurePath);
        }
    }
}
