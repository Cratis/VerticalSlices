// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.EventModelAdvisory.Rules;

/// <summary>
/// Detects read model event-to-property mappings that specify a mapping kind requiring
/// a source property name but have <see cref="EventPropertyMapping.SourcePropertyName"/>
/// set to <see langword="null"/> or empty.
/// The following kinds require a source property name:
/// <list type="bullet">
///   <item><see cref="EventPropertyMappingKind.Set"/> — name of the event property to copy.</item>
///   <item><see cref="EventPropertyMappingKind.Add"/> — name of the numeric event property to add.</item>
///   <item><see cref="EventPropertyMappingKind.Subtract"/> — name of the numeric event property to subtract.</item>
///   <item><see cref="EventPropertyMappingKind.SetFromContext"/> — name of the event context field (e.g. "Occurred").</item>
///   <item><see cref="EventPropertyMappingKind.StaticValue"/> — the literal static value to assign.</item>
/// </list>
/// A missing source property will produce a mapping that the code generator cannot fulfil.
/// </summary>
public class MissingSourcePropertyInMappingRule : IEventModelRule
{
    static readonly HashSet<EventPropertyMappingKind> _kindsRequiringSource =
    [
        EventPropertyMappingKind.Set,
        EventPropertyMappingKind.Add,
        EventPropertyMappingKind.Subtract,
        EventPropertyMappingKind.SetFromContext,
        EventPropertyMappingKind.StaticValue
    ];

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
                    foreach (var property in readModel.Properties)
                    {
                        foreach (var mapping in property.Mappings.Where(NeedsSourceButIsMissing))
                        {
                            yield return new EventModelRecommendation(
                                EventModelRecommendationSeverity.Error,
                                EventModelRecommendationCategory.Coverage,
                                moduleName,
                                featurePath,
                                slice.Name,
                                readModel.Name,
                                $"Read model property '{property.Name}' on '{readModel.Name}' has a '{mapping.Kind}' mapping from event '{mapping.EventTypeName}' but no source property name is specified.",
                                $"Set the source property name for the '{mapping.Kind}' mapping to tell the projection which value to read.");
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

    static bool NeedsSourceButIsMissing(EventPropertyMapping mapping) =>
        _kindsRequiringSource.Contains(mapping.Kind) && string.IsNullOrWhiteSpace(mapping.SourcePropertyName);
}
