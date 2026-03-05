// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.DependencyInjection;

namespace Cratis.VerticalSlices.EventModelAdvisory.Rules;

/// <summary>
/// Detects commands whose explicit <see cref="Command.ProducedEvents"/> list references
/// an event type name that is not defined in the containing slice's events collection.
/// </summary>
[Singleton]
public class ProducedEventNotDefinedRule : IEventModelRule
{
    /// <inheritdoc/>
    public IEnumerable<EventModelRecommendation> Evaluate(IEnumerable<Module> modules)
    {
        foreach (var (moduleName, path, slice) in modules.FlattenSlices())
        {
            var definedEventNames = slice.Events
                .Select(e => e.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var command in slice.Commands.Where(c => c.ProducedEvents is not null))
            {
                foreach (var produced in command.ProducedEvents!)
                {
                    if (!definedEventNames.Contains(produced.EventTypeName))
                    {
                        yield return new EventModelRecommendation(
                            EventModelRecommendationSeverity.Error,
                            EventModelRecommendationCategory.Coverage,
                            moduleName,
                            path,
                            slice.Name,
                            command.Name,
                            $"Command '{command.Name}' declares it produces event '{produced.EventTypeName}', but that event type is not defined in the slice.",
                            $"Add an event type named '{produced.EventTypeName}' to the slice, or remove it from the command's produced events list.");
                    }
                }
            }
        }
    }
}
