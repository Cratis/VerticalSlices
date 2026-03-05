// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.DependencyInjection;

namespace Cratis.VerticalSlices.EventModelAdvisory.Rules;

/// <summary>
/// Detects events that are defined in a module but never referenced by any
/// read model property mapping. These orphaned events produce data that
/// no projection consumes locally.
/// </summary>
[Singleton]
public class OrphanedEventRule : IEventModelRule
{
    /// <inheritdoc/>
    public IEnumerable<EventModelRecommendation> Evaluate(IEnumerable<Module> modules)
    {
        var allSlices = modules.FlattenSlices().ToList();

        var referencedEvents = allSlices
            .SelectMany(item => item.Slice.ReadModels)
            .SelectMany(rm => rm.Properties)
            .SelectMany(p => p.Mappings)
            .Select(m => m.EventTypeName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var (moduleName, path, slice) in allSlices)
        {
            foreach (var eventType in slice.Events.Where(e => !referencedEvents.Contains(e.Name)))
            {
                yield return new EventModelRecommendation(
                    EventModelRecommendationSeverity.Suggestion,
                    EventModelRecommendationCategory.Coverage,
                    moduleName,
                    path,
                    slice.Name,
                    eventType.Name,
                    $"Event type '{eventType.Name}' is not referenced by any read model property mapping.",
                    "If this event is consumed by an external system or a different module, consider documenting that relationship. Otherwise, add a read model that projects this event.");
            }
        }
    }
}
