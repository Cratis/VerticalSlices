// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.DependencyInjection;

namespace Cratis.VerticalSlices.EventModelAdvisory.Rules;

/// <summary>
/// Detects <see cref="VerticalSliceType.StateChange"/> slices that have no events.
/// A StateChange slice must produce at least one event to record a domain side-effect.
/// </summary>
[Singleton]
public class StateChangeWithNoEventsRule : IEventModelRule
{
    /// <inheritdoc/>
    public IEnumerable<EventModelRecommendation> Evaluate(IEnumerable<Module> modules)
    {
        foreach (var (moduleName, path, slice) in modules.FlattenSlices()
            .Where(item => item.Slice.SliceType == VerticalSliceType.StateChange && !item.Slice.Events.Any()))
        {
            yield return new EventModelRecommendation(
                EventModelRecommendationSeverity.Error,
                EventModelRecommendationCategory.Structure,
                moduleName,
                path,
                slice.Name,
                string.Empty,
                $"StateChange slice '{slice.Name}' has no events. A StateChange slice must produce at least one event to record a domain side-effect.",
                "Add at least one event type to the slice, or change the slice type if no domain event should be recorded.");
        }
    }
}
