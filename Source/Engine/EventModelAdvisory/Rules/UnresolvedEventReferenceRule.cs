// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.EventModelAdvisory.Rules;

/// <summary>
/// Detects read model property mappings that reference event type names not defined
/// anywhere in the module. An unresolvable event reference will cause projection code
/// generation to fail or emit broken output — this must be treated as a hard error.
/// </summary>
/// <remarks>
/// Event references are resolved against all events across the entire module so that
/// read models in Automation or StateView slices can subscribe to events produced
/// by other slices in the same module. The wildcard <c>*</c> is always valid and
/// is never flagged.
/// </remarks>
public class UnresolvedEventReferenceRule : IEventModelRule
{
    /// <inheritdoc/>
    public IEnumerable<EventModelRecommendation> Evaluate(IEnumerable<Module> modules)
    {
        foreach (var module in modules)
        {
            var allModuleEventNames = CollectAllEventNames(module.Features);

            foreach (var recommendation in EvaluateFeatures(module.Features, module.Name, FeaturePath.Empty, allModuleEventNames))
            {
                yield return recommendation;
            }
        }
    }

    static IReadOnlySet<string> CollectAllEventNames(IEnumerable<Feature> features)
    {
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        CollectEventNamesFromFeatures(features, names);
        return names;
    }

    static void CollectEventNamesFromFeatures(IEnumerable<Feature> features, HashSet<string> names)
    {
        foreach (var feature in features)
        {
            foreach (var slice in feature.VerticalSlices)
            {
                foreach (var e in slice.Events)
                {
                    names.Add(e.Name);
                }
            }

            CollectEventNamesFromFeatures(feature.Features, names);
        }
    }

    static IEnumerable<EventModelRecommendation> EvaluateFeatures(IEnumerable<Feature> features, string moduleName, FeaturePath path, IReadOnlySet<string> allModuleEventNames)
    {
        foreach (var feature in features)
        {
            var featurePath = path.Append(feature.Name);
            foreach (var slice in feature.VerticalSlices)
            {
                foreach (var readModel in slice.ReadModels)
                {
                    var referencedNames = readModel.Properties
                        .SelectMany(p => p.Mappings)
                        .Select(m => m.EventTypeName)
                        .Distinct(StringComparer.OrdinalIgnoreCase);

                    foreach (var referencedName in referencedNames.Where(name =>
                        name != "*" &&
                        !allModuleEventNames.Contains(name)))
                    {
                        yield return new EventModelRecommendation(
                            EventModelRecommendationSeverity.Error,
                            EventModelRecommendationCategory.Coverage,
                            moduleName,
                            featurePath,
                            slice.Name,
                            readModel.Name,
                            $"Read model '{readModel.Name}' references event type '{referencedName}' which is not defined anywhere in the module.",
                            $"Add an event type named '{referencedName}' to the appropriate slice in the module, or correct the mapping if this is a typo.");
                    }
                }
            }

            foreach (var recommendation in EvaluateFeatures(feature.Features, moduleName, featurePath, allModuleEventNames))
            {
                yield return recommendation;
            }
        }
    }
}
