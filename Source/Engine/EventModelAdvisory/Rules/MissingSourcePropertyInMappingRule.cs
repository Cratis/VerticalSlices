// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.DependencyInjection;

namespace Cratis.VerticalSlices.EventModelAdvisory.Rules;

/// <summary>
/// Detects read model event-to-property mappings that specify a mapping kind requiring
/// a source property name but have <see cref="EventPropertyMapping.SourcePropertyName"/>
/// set to <see langword="null"/> or empty.
/// </summary>
[Singleton]
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
        foreach (var (moduleName, path, slice) in modules.FlattenSlices())
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
                            path,
                            slice.Name,
                            readModel.Name,
                            $"Read model property '{property.Name}' on '{readModel.Name}' has a '{mapping.Kind}' mapping from event '{mapping.EventTypeName}' but no source property name is specified.",
                            $"Set the source property name for the '{mapping.Kind}' mapping to tell the projection which value to read.");
                    }
                }
            }
        }
    }

    static bool NeedsSourceButIsMissing(EventPropertyMapping mapping) =>
        _kindsRequiringSource.Contains(mapping.Kind) && string.IsNullOrWhiteSpace(mapping.SourcePropertyName);
}
