// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.EventModelAdvisory.Rules;

/// <summary>
/// Detects event types with a high number of properties. Events with too many
/// properties may be trying to capture too many facts at once, violating the
/// single-responsibility principle of event-driven design.
/// </summary>
public class EventWithTooManyPropertiesRule : IEventModelRule
{
    /// <summary>
    /// The maximum number of properties an event should have before a recommendation is raised.
    /// </summary>
    public const int MaxRecommendedProperties = 5;

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
                foreach (var eventType in slice.Events.Where(e => e.Properties.Count() > MaxRecommendedProperties))
                {
                    var count = eventType.Properties.Count();
                    yield return new EventModelRecommendation(
                        EventModelRecommendationSeverity.Suggestion,
                        EventModelRecommendationCategory.Structure,
                        moduleName,
                        featurePath,
                        slice.Name,
                        eventType.Name,
                        $"Event type '{eventType.Name}' has {count} properties, which exceeds the recommended maximum of {MaxRecommendedProperties}.",
                        "Consider splitting this event into smaller, more focused events that each capture a single fact or closely related set of facts.");
                }
            }

            foreach (var recommendation in EvaluateFeatures(feature.Features, moduleName, featurePath))
            {
                yield return recommendation;
            }
        }
    }
}
