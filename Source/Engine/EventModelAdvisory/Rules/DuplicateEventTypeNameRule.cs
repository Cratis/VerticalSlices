// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.DependencyInjection;

namespace Cratis.VerticalSlices.EventModelAdvisory.Rules;

/// <summary>
/// Detects event types that are produced by more than one StateChange or Automation slice.
/// Two independent commands claiming to produce the same named event is usually a modeling mistake.
/// </summary>
[Singleton]
public class DuplicateEventTypeNameRule : IEventModelRule
{
    /// <inheritdoc/>
    public IEnumerable<EventModelRecommendation> Evaluate(IEnumerable<Module> modules)
    {
        var occurrences = modules.FlattenSlices()
            .Where(item => IsProducingSlice(item.Slice))
            .SelectMany(item => item.Slice.Events.Select(e => (SliceName: item.Slice.Name, EventName: e.Name)))
            .ToList();

        var duplicates = occurrences
            .GroupBy(e => e.EventName, StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1);

        foreach (var group in duplicates)
        {
            var sliceNames = string.Join(", ", group.Select(e => $"'{e.SliceName}'"));
            yield return new EventModelRecommendation(
                EventModelRecommendationSeverity.Warning,
                EventModelRecommendationCategory.Structure,
                string.Empty,
                FeaturePath.Empty,
                string.Empty,
                group.Key,
                $"Event type '{group.Key}' is produced by multiple slices: {sliceNames}.",
                "Each event type should be produced by exactly one slice. Consider renaming to reflect distinct intent.");
        }
    }

    static bool IsProducingSlice(VerticalSlice s) =>
        s.SliceType is VerticalSliceType.StateChange or VerticalSliceType.Automation;
}
