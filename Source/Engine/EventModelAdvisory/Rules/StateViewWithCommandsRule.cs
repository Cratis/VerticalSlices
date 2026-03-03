// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.DependencyInjection;

namespace Cratis.VerticalSlices.EventModelAdvisory.Rules;

/// <summary>
/// Detects commands placed inside a <see cref="VerticalSliceType.StateView"/> slice.
/// StateView slices project events into read models; issuing commands from a StateView
/// is an architectural violation and will result in incorrect code generation.
/// </summary>
[Singleton]
public class StateViewWithCommandsRule : IEventModelRule
{
    /// <inheritdoc/>
    public IEnumerable<EventModelRecommendation> Evaluate(IEnumerable<Module> modules)
    {
        foreach (var (moduleName, path, slice) in modules.FlattenSlices()
            .Where(item => item.Slice.SliceType == VerticalSliceType.StateView))
        {
            foreach (var command in slice.Commands)
            {
                yield return new EventModelRecommendation(
                    EventModelRecommendationSeverity.Error,
                    EventModelRecommendationCategory.Structure,
                    moduleName,
                    path,
                    slice.Name,
                    command.Name,
                    $"StateView slice '{slice.Name}' contains command '{command.Name}'. StateView slices project events into read models and do not issue commands.",
                    "Move the command to a StateChange or Automation slice, or change the slice type if the intent is to capture user intent.");
            }
        }
    }
}
