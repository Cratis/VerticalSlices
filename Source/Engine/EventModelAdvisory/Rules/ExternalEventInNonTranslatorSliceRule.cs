// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.DependencyInjection;

namespace Cratis.VerticalSlices.EventModelAdvisory.Rules;

/// <summary>
/// Detects external events used in slices other than <see cref="VerticalSliceType.Translator"/>.
/// External events can only be consumed in Translator slices; using one in any other slice type
/// will lead to incorrect code generation.
/// </summary>
[Singleton]
public class ExternalEventInNonTranslatorSliceRule : IEventModelRule
{
    /// <inheritdoc/>
    public IEnumerable<EventModelRecommendation> Evaluate(IEnumerable<Module> modules)
    {
        foreach (var (moduleName, path, slice) in modules.FlattenSlices()
            .Where(item => item.Slice.SliceType != VerticalSliceType.Translator))
        {
            foreach (var externalEvent in slice.Events.Where(e => e.Kind == EventKind.External))
            {
                yield return new EventModelRecommendation(
                    EventModelRecommendationSeverity.Error,
                    EventModelRecommendationCategory.Structure,
                    moduleName,
                    path,
                    slice.Name,
                    externalEvent.Name,
                    $"Event '{externalEvent.Name}' is External and cannot be used in a {slice.SliceType} slice. External events are only valid in Translator slices.",
                    "Move the external event to a Translator slice, or change the event kind to Internal if it originates within this bounded context.");
            }
        }
    }
}
