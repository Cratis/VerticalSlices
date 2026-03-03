// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.DependencyInjection;

namespace Cratis.VerticalSlices.EventModelAdvisory.Rules;

/// <summary>
/// Detects <see cref="VerticalSliceType.StateView"/> slices that define no read models.
/// A StateView without read models has no projection target.
/// </summary>
[Singleton]
public class StateViewWithNoReadModelsRule : IEventModelRule
{
    /// <inheritdoc/>
    public IEnumerable<EventModelRecommendation> Evaluate(IEnumerable<Module> modules)
    {
        foreach (var (moduleName, path, slice) in modules.FlattenSlices()
            .Where(item => item.Slice.SliceType == VerticalSliceType.StateView && !item.Slice.ReadModels.Any()))
        {
            yield return new EventModelRecommendation(
                EventModelRecommendationSeverity.Warning,
                EventModelRecommendationCategory.Structure,
                moduleName,
                path,
                slice.Name,
                slice.Name,
                $"StateView slice '{slice.Name}' has no read models. A StateView projects events into read models displayed on a screen.",
                "Add at least one read model to define what data this slice projects and exposes.");
        }
    }
}
