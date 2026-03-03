// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.DependencyInjection;

namespace Cratis.VerticalSlices.EventModelAdvisory.Rules;

/// <summary>
/// Detects read models that do not have any property mapped from the event source identifier.
/// Most read models need a key property to identify the aggregate instance being projected.
/// </summary>
[Singleton]
public class ReadModelWithoutKeyMappingRule : IEventModelRule
{
    /// <inheritdoc/>
    public IEnumerable<EventModelRecommendation> Evaluate(IEnumerable<Module> modules)
    {
        foreach (var (moduleName, path, slice) in modules.FlattenSlices())
        {
            foreach (var readModel in slice.ReadModels)
            {
                var hasKeyMapping = readModel.Properties
                    .SelectMany(p => p.Mappings)
                    .Any(m => m.Kind == EventPropertyMappingKind.FromEventSourceId);

                if (!hasKeyMapping && readModel.Properties.Any())
                {
                    yield return new EventModelRecommendation(
                        EventModelRecommendationSeverity.Suggestion,
                        EventModelRecommendationCategory.BestPractice,
                        moduleName,
                        path,
                        slice.Name,
                        readModel.Name,
                        $"Read model '{readModel.Name}' does not have a property mapped from the event source identifier.",
                        "Add a key property (e.g. 'Id') with a FromEventSourceId mapping so the projection can identify the aggregate instance.");
                }
            }
        }
    }
}
