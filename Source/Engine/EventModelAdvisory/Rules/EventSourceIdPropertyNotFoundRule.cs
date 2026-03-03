// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.DependencyInjection;

namespace Cratis.VerticalSlices.EventModelAdvisory.Rules;

/// <summary>
/// Detects commands where <see cref="EventSourceIdStrategy.Supplied"/> is used but the
/// value of <see cref="Command.EventSourceId"/> does not match any property name declared
/// in <see cref="Command.Properties"/>.
/// </summary>
[Singleton]
public class EventSourceIdPropertyNotFoundRule : IEventModelRule
{
    /// <inheritdoc/>
    public IEnumerable<EventModelRecommendation> Evaluate(IEnumerable<Module> modules)
    {
        foreach (var (moduleName, path, slice) in modules.FlattenSlices())
        {
            foreach (var command in slice.Commands.Where(c => c.EventSourceIdStrategy == EventSourceIdStrategy.Supplied))
            {
                var properties = command.Properties.ToList();
                if (properties.Count == 0)
                {
                    continue;
                }

                if (!properties.Exists(p => string.Equals(p.Name, command.EventSourceId, StringComparison.OrdinalIgnoreCase)))
                {
                    yield return new EventModelRecommendation(
                        EventModelRecommendationSeverity.Warning,
                        EventModelRecommendationCategory.Structure,
                        moduleName,
                        path,
                        slice.Name,
                        command.Name,
                        $"Command '{command.Name}' uses EventSourceIdStrategy.Supplied with EventSourceId '{command.EventSourceId}', but no property with that name exists on the command.",
                        $"Add a property named '{command.EventSourceId}' to the command, or verify that it is supplied via a route parameter.");
                }
            }
        }
    }
}
