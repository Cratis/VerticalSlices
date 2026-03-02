// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.EventModelAdvisory.Rules;

/// <summary>
/// Detects commands where <see cref="EventSourceIdStrategy.Supplied"/> is used but the
/// value of <see cref="Command.EventSourceId"/> does not match any property name declared
/// in <see cref="Command.Properties"/>.
/// When the strategy is <see cref="EventSourceIdStrategy.Supplied"/> the caller must
/// provide the event source identifier as one of the command properties; if no such
/// property exists the generated command handler cannot bind the identifier and will
/// fail to compile or behave incorrectly at runtime.
/// </summary>
public class EventSourceIdPropertyNotFoundRule : IEventModelRule
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
                foreach (var command in slice.Commands)
                {
                    if (command.EventSourceIdStrategy != EventSourceIdStrategy.Supplied)
                    {
                        continue;
                    }

                    var properties = command.Properties.ToList();
                    if (properties.Count == 0)
                    {
                        continue;
                    }

                    var hasMatchingProperty = properties.Exists(p =>
                        string.Equals(p.Name, command.EventSourceId, StringComparison.OrdinalIgnoreCase));

                    if (!hasMatchingProperty)
                    {
                        yield return new EventModelRecommendation(
                            EventModelRecommendationSeverity.Error,
                            EventModelRecommendationCategory.Structure,
                            moduleName,
                            featurePath,
                            slice.Name,
                            command.Name,
                            $"Command '{command.Name}' uses the Supplied event source id strategy but has no property named '{command.EventSourceId}'.",
                            $"Add a property named '{command.EventSourceId}' to the command so callers can supply the event source identifier.");
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
