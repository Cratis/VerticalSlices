// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.DependencyInjection;

namespace Cratis.VerticalSlices.EventModelAdvisory.Rules;

/// <summary>
/// Detects read model property mappings that reference event type names not defined
/// anywhere in the module. An unresolvable event reference will cause projection code
/// generation to fail or emit broken output.
/// </summary>
/// <remarks>
/// Event references are resolved against all events across the entire module so that
/// read models in Automation or StateView slices can subscribe to events produced
/// by other slices in the same module. The wildcard <c>*</c> is always valid and
/// is never flagged.
/// </remarks>
[Singleton]
public class UnresolvedEventReferenceRule : IEventModelRule
{
    /// <inheritdoc/>
    public IEnumerable<EventModelRecommendation> Evaluate(IEnumerable<Module> modules)
    {
        foreach (var module in modules)
        {
            var allModuleEventNames = module.Features
                .FlattenSlices(module.Name)
                .SelectMany(item => item.Slice.Events)
                .Select(e => e.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var (_, path, slice) in module.Features.FlattenSlices(module.Name))
            {
                foreach (var readModel in slice.ReadModels)
                {
                    var referencedNames = readModel.Properties
                        .SelectMany(p => p.Mappings)
                        .Select(m => m.EventTypeName)
                        .Distinct(StringComparer.OrdinalIgnoreCase);

                    foreach (var referencedName in referencedNames.Where(name =>
                        name != "*" && !allModuleEventNames.Contains(name)))
                    {
                        yield return new EventModelRecommendation(
                            EventModelRecommendationSeverity.Error,
                            EventModelRecommendationCategory.Coverage,
                            module.Name,
                            path,
                            slice.Name,
                            readModel.Name,
                            $"Read model '{readModel.Name}' references event type '{referencedName}' which is not defined anywhere in the module.",
                            $"Add an event type named '{referencedName}' to the appropriate slice in the module, or correct the mapping if this is a typo.");
                    }
                }
            }
        }
    }
}
