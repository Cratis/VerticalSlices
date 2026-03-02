// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.EventModelAdvisory.Rules;

/// <summary>
/// Detects properties on events, commands, and read models that use primitive types
/// instead of domain concepts. Using concepts provides stronger typing, reusability,
/// and built-in validation.
/// </summary>
public class PrimitivePropertyTypeRule : IEventModelRule
{
    static readonly HashSet<string> _primitiveTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "string",
        "int",
        "long",
        "short",
        "byte",
        "float",
        "double",
        "decimal",
        "bool",
        "Guid",
        "DateTime",
        "DateTimeOffset",
        "DateOnly",
        "TimeOnly"
    };

    /// <inheritdoc/>
    public IEnumerable<EventModelRecommendation> Evaluate(IEnumerable<Module> modules)
    {
        foreach (var module in modules)
        {
            var conceptNames = new HashSet<string>(
                module.Concepts.Select(c => c.Name),
                StringComparer.OrdinalIgnoreCase);

            foreach (var recommendation in EvaluateFeatures(module.Features, conceptNames, module.Name, FeaturePath.Empty))
            {
                yield return recommendation;
            }
        }
    }

    static IEnumerable<EventModelRecommendation> EvaluateFeatures(IEnumerable<Feature> features, HashSet<string> conceptNames, string moduleName, FeaturePath path)
    {
        foreach (var feature in features)
        {
            var featurePath = path.Append(feature.Name);
            foreach (var concept in feature.Concepts)
            {
                conceptNames.Add(concept.Name);
            }

            foreach (var slice in feature.VerticalSlices)
            {
                foreach (var eventType in slice.Events)
                {
                    foreach (var property in eventType.Properties.Where(p => IsPrimitive(p.Type)))
                    {
                        yield return new EventModelRecommendation(
                            EventModelRecommendationSeverity.Suggestion,
                            EventModelRecommendationCategory.BestPractice,
                            moduleName,
                            featurePath,
                            slice.Name,
                            eventType.Name,
                            $"Event property '{property.Name}' on '{eventType.Name}' uses primitive type '{property.Type}'.",
                            $"Consider creating a domain concept for '{property.Name}' to provide stronger typing and reusable validation.");
                    }
                }

                foreach (var command in slice.Commands)
                {
                    foreach (var property in command.Properties.Where(p => IsPrimitive(p.Type)))
                    {
                        yield return new EventModelRecommendation(
                            EventModelRecommendationSeverity.Suggestion,
                            EventModelRecommendationCategory.BestPractice,
                            moduleName,
                            featurePath,
                            slice.Name,
                            command.Name,
                            $"Command property '{property.Name}' on '{command.Name}' uses primitive type '{property.Type}'.",
                            $"Consider creating a domain concept for '{property.Name}' to provide stronger typing and reusable validation.");
                    }
                }
            }

            foreach (var recommendation in EvaluateFeatures(feature.Features, conceptNames, moduleName, featurePath))
            {
                yield return recommendation;
            }
        }
    }

    static bool IsPrimitive(string typeName) => _primitiveTypes.Contains(typeName);
}
