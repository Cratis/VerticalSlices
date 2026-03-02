// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.EventModelAdvisory.Rules;

/// <summary>
/// Detects read model property mappings that reference event type names not defined
/// in the same slice. An unresolvable event reference will cause projection code generation
/// to fail or emit broken output — this must be treated as a hard error.
/// </summary>
public class UnresolvedEventReferenceRule : IEventModelRule
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

                foreach (var readModel in slice.ReadModels)
                {
                    var referencedNames = readModel.Properties
                        .SelectMany(p => p.Mappings)
                        .Select(m => m.EventTypeName)
                        .Distinct(StringComparer.OrdinalIgnoreCase);

                    foreach (var referencedName in referencedNames.Where(name => !definedEventNames.Contains(name)))
                    {
                        yield return new EventModelRecommendation(
                            EventModelRecommendationSeverity.Error,
                            EventModelRecommendationCategory.Coverage,
                            moduleName,
                            featurePath,
                            slice.Name,
                            readModel.Name,
                            $"Read model '{readModel.Name}' references event type '{referencedName}' which is not defined in the slice.",
                            $"Add an event type named '{referencedName}' to the slice, or correct the mapping if this is a typo.");
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
