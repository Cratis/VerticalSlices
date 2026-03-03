// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.DependencyInjection;

namespace Cratis.VerticalSlices.EventModelAdvisory.Rules;

/// <summary>
/// Detects properties on events and commands that use primitive types
/// instead of domain concepts. Using concepts provides stronger typing, reusability,
/// and built-in validation.
/// </summary>
[Singleton]
public class PrimitivePropertyTypeRule : IEventModelRule
{
    static readonly HashSet<string> _primitiveTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "string", "int", "long", "short", "byte", "float", "double", "decimal",
        "bool", "Guid", "DateTime", "DateTimeOffset", "DateOnly", "TimeOnly"
    };

    /// <inheritdoc/>
    public IEnumerable<EventModelRecommendation> Evaluate(IEnumerable<Module> modules)
    {
        foreach (var (moduleName, path, slice) in modules.FlattenSlices())
        {
            foreach (var eventType in slice.Events)
            {
                foreach (var property in eventType.Properties.Where(p => IsPrimitive(p.Type)))
                {
                    yield return new EventModelRecommendation(
                        EventModelRecommendationSeverity.Suggestion,
                        EventModelRecommendationCategory.BestPractice,
                        moduleName,
                        path,
                        slice.Name,
                        eventType.Name,
                        $"Event property '{property.Name}' on '{eventType.Name}' uses primitive type '{property.Type}'.",
                        $"Consider creating a domain concept for '{property.Name}' to provide stronger typing and reusable validation.");
                }
            }

            foreach (var command in slice.Commands)
            {
                foreach (var property in command.Properties.Where(p => IsPrimitive(p.Type)))
                {
                    yield return new EventModelRecommendation(
                        EventModelRecommendationSeverity.Suggestion,
                        EventModelRecommendationCategory.BestPractice,
                        moduleName,
                        path,
                        slice.Name,
                        command.Name,
                        $"Command property '{property.Name}' on '{command.Name}' uses primitive type '{property.Type}'.",
                        $"Consider creating a domain concept for '{property.Name}' to provide stronger typing and reusable validation.");
                }
            }
        }
    }

    static bool IsPrimitive(string typeName) => _primitiveTypes.Contains(typeName);
}
