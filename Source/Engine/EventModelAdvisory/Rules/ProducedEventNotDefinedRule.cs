// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.EventModelAdvisory.Rules;

/// <summary>
/// Detects commands whose explicit <see cref="Command.ProducedEvents"/> list references
/// an event type name that is not defined in the containing slice's <see cref="VerticalSlice.Events"/>
/// collection. An unresolvable produced-event reference will cause code generation to fail
/// or emit broken output.
/// </summary>
public class ProducedEventNotDefinedRule : IEventModelRule
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
                var definedEventNames = new HashSet<string>(
                    slice.Events.Select(e => e.Name),
                    StringComparer.OrdinalIgnoreCase);

                foreach (var command in slice.Commands.Where(c => c.ProducedEvents is not null))
                {
                    foreach (var produced in command.ProducedEvents!)
                    {
                        if (!definedEventNames.Contains(produced.EventTypeName))
                        {
                            yield return new EventModelRecommendation(
                                EventModelRecommendationSeverity.Error,
                                EventModelRecommendationCategory.Coverage,
                                moduleName,
                                featurePath,
                                slice.Name,
                                command.Name,
                                $"Command '{command.Name}' declares it produces event '{produced.EventTypeName}', but that event type is not defined in the slice.",
                                $"Add an event type named '{produced.EventTypeName}' to the slice, or remove it from the command's produced events list.");
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
