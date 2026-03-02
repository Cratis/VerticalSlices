// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.EventModelAdvisory.Rules;

/// <summary>
/// Detects event types that are produced (i.e. listed in the <see cref="VerticalSlice.Events"/>
/// collection) by more than one <see cref="VerticalSliceType.StateChange"/> or
/// <see cref="VerticalSliceType.Automation"/> slice.
/// It is perfectly normal for StateView slices to consume the same event via read model
/// property mappings; this rule only cares about event production, where two independent
/// commands claiming to produce the same named event is usually a modeling mistake.
/// </summary>
public class DuplicateEventTypeNameRule : IEventModelRule
{
    /// <inheritdoc/>
    public IEnumerable<EventModelRecommendation> Evaluate(IEnumerable<Module> modules)
    {
        var producingOccurrences = new List<(string SliceName, string EventName)>();
        foreach (var module in modules)
        {
            CollectFromFeatures(module.Features, producingOccurrences);
        }

        var duplicates = producingOccurrences
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
                "Each event type should be produced by exactly one slice. If multiple commands need to produce the same fact, consider whether they should share a single command or be renamed to reflect their distinct intent.");
        }
    }

    static void CollectFromFeatures(IEnumerable<Feature> features, List<(string SliceName, string EventName)> occurrences)
    {
        foreach (var feature in features)
        {
            foreach (var slice in feature.VerticalSlices.Where(IsProducingSlice))
            {
                foreach (var eventType in slice.Events)
                {
                    occurrences.Add((slice.Name, eventType.Name));
                }
            }

            CollectFromFeatures(feature.Features, occurrences);
        }
    }

    static bool IsProducingSlice(VerticalSlice slice) =>
        slice.SliceType is VerticalSliceType.StateChange or VerticalSliceType.Automation;
}
