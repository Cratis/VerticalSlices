// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.DependencyInjection;

namespace Cratis.VerticalSlices.EventModelAdvisory.Rules;

/// <summary>
/// Detects <see cref="VerticalSliceType.StateChange"/> slices that have no commands.
/// A StateChange slice exists to capture user intent; without at least one command there
/// is no way to express that intent, making the slice structurally incomplete.
/// </summary>
[Singleton]
public class StateChangeWithNoCommandsRule : IEventModelRule
{
    /// <inheritdoc/>
    public IEnumerable<EventModelRecommendation> Evaluate(IEnumerable<Module> modules)
    {
        foreach (var (moduleName, path, slice) in modules.FlattenSlices()
            .Where(item => item.Slice.SliceType == VerticalSliceType.StateChange && !item.Slice.Commands.Any()))
        {
            yield return new EventModelRecommendation(
                EventModelRecommendationSeverity.Error,
                EventModelRecommendationCategory.Structure,
                moduleName,
                path,
                slice.Name,
                string.Empty,
                $"StateChange slice '{slice.Name}' has no commands. A StateChange slice must have at least one command to capture user intent.",
                "Add at least one command to the slice, or change the slice type if no user intent needs to be captured.");
        }
    }
}
