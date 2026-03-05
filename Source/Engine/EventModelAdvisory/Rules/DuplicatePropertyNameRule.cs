// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.DependencyInjection;

namespace Cratis.VerticalSlices.EventModelAdvisory.Rules;

/// <summary>
/// Detects duplicate property names within a single event type, command, or read model.
/// Duplicate names within one artifact will produce uncompilable generated code.
/// </summary>
[Singleton]
public class DuplicatePropertyNameRule : IEventModelRule
{
    /// <inheritdoc/>
    public IEnumerable<EventModelRecommendation> Evaluate(IEnumerable<Module> modules)
    {
        foreach (var (moduleName, path, slice) in modules.FlattenSlices())
        {
            foreach (var eventType in slice.Events)
            {
                foreach (var duplicate in FindDuplicates(eventType.Properties.Select(p => p.Name)))
                {
                    yield return new EventModelRecommendation(
                        EventModelRecommendationSeverity.Error,
                        EventModelRecommendationCategory.Structure,
                        moduleName,
                        path,
                        slice.Name,
                        eventType.Name,
                        $"Event type '{eventType.Name}' has duplicate property name '{duplicate}'.",
                        "Remove or rename the duplicate property. Each property name must be unique within an event type.");
                }
            }

            foreach (var command in slice.Commands)
            {
                foreach (var duplicate in FindDuplicates(command.Properties.Select(p => p.Name)))
                {
                    yield return new EventModelRecommendation(
                        EventModelRecommendationSeverity.Error,
                        EventModelRecommendationCategory.Structure,
                        moduleName,
                        path,
                        slice.Name,
                        command.Name,
                        $"Command '{command.Name}' has duplicate property name '{duplicate}'.",
                        "Remove or rename the duplicate property. Each property name must be unique within a command.");
                }
            }

            foreach (var readModel in slice.ReadModels)
            {
                foreach (var duplicate in FindDuplicates(readModel.Properties.Select(p => p.Name)))
                {
                    yield return new EventModelRecommendation(
                        EventModelRecommendationSeverity.Error,
                        EventModelRecommendationCategory.Structure,
                        moduleName,
                        path,
                        slice.Name,
                        readModel.Name,
                        $"Read model '{readModel.Name}' has duplicate property name '{duplicate}'.",
                        "Remove or rename the duplicate property. Each property name must be unique within a read model.");
                }
            }
        }
    }

    static IEnumerable<string> FindDuplicates(IEnumerable<string> names) =>
        names
            .GroupBy(n => n, StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key);
}
