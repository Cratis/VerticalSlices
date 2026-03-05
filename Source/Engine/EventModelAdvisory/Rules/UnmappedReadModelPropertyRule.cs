// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.DependencyInjection;

namespace Cratis.VerticalSlices.EventModelAdvisory.Rules;

/// <summary>
/// Detects read model properties that have no event-to-property mappings defined.
/// Properties without mappings will not be populated by the projection.
/// </summary>
[Singleton]
public class UnmappedReadModelPropertyRule : IEventModelRule
{
    /// <inheritdoc/>
    public IEnumerable<EventModelRecommendation> Evaluate(IEnumerable<Module> modules)
    {
        foreach (var (moduleName, path, slice) in modules.FlattenSlices())
        {
            foreach (var readModel in slice.ReadModels)
            {
                foreach (var property in readModel.Properties.Where(p => !p.Mappings.Any()))
                {
                    yield return new EventModelRecommendation(
                        EventModelRecommendationSeverity.Information,
                        EventModelRecommendationCategory.Coverage,
                        moduleName,
                        path,
                        slice.Name,
                        readModel.Name,
                        $"Read model property '{property.Name}' on '{readModel.Name}' has no event mappings.",
                        "Add an event-to-property mapping so the projection populates this property, or remove it if it is not needed.");
                }
            }
        }
    }
}
