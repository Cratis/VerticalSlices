// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.EventModelAdvisory.Rules;

/// <summary>
/// Detects duplicate artifact names within a single slice: two commands, two read models,
/// or two event types sharing the same name. Duplicates within a slice prevent code
/// generation from producing valid, compilable output.
/// </summary>
public class DuplicateArtifactNameInSliceRule : IEventModelRule
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
                foreach (var duplicate in FindDuplicates(slice.Commands.Select(c => c.Name)))
                {
                    yield return new EventModelRecommendation(
                        EventModelRecommendationSeverity.Error,
                        EventModelRecommendationCategory.Structure,
                        moduleName,
                        featurePath,
                        slice.Name,
                        duplicate,
                        $"Slice '{slice.Name}' contains more than one command named '{duplicate}'.",
                        "Each command within a slice must have a unique name.");
                }

                foreach (var duplicate in FindDuplicates(slice.ReadModels.Select(r => r.Name)))
                {
                    yield return new EventModelRecommendation(
                        EventModelRecommendationSeverity.Error,
                        EventModelRecommendationCategory.Structure,
                        moduleName,
                        featurePath,
                        slice.Name,
                        duplicate,
                        $"Slice '{slice.Name}' contains more than one read model named '{duplicate}'.",
                        "Each read model within a slice must have a unique name.");
                }

                foreach (var duplicate in FindDuplicates(slice.Events.Select(e => e.Name)))
                {
                    yield return new EventModelRecommendation(
                        EventModelRecommendationSeverity.Error,
                        EventModelRecommendationCategory.Structure,
                        moduleName,
                        featurePath,
                        slice.Name,
                        duplicate,
                        $"Slice '{slice.Name}' contains more than one event type named '{duplicate}'.",
                        "Each event type within a slice must have a unique name.");
                }
            }

            foreach (var recommendation in EvaluateFeatures(feature.Features, moduleName, featurePath))
            {
                yield return recommendation;
            }
        }
    }

    static IEnumerable<string> FindDuplicates(IEnumerable<string> names) =>
        names
            .GroupBy(n => n, StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key);
}
