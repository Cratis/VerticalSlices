// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.DependencyInjection;
using Cratis.VerticalSlices.EventModelAdvisory.Rules;

namespace Cratis.VerticalSlices.EventModelAdvisory;

/// <summary>
/// Analyzes an event model across all modules and produces recommendations
/// for improving the model structure, naming, coverage, and best practices.
/// </summary>
[Singleton]
public class EventModelAdvisor : IEventModelAdvisor
{
    readonly IEventModelRule[] _rules =
    [
        // Error-level rules — must be fixed before code generation can succeed
        new DuplicatePropertyNameRule(),
        new DuplicateArtifactNameInSliceRule(),
        new ProducedEventNotDefinedRule(),
        new MissingSourcePropertyInMappingRule(),
        new UnresolvedEventReferenceRule(),
        new EventPropertyMappingSourcePropertyNotFoundRule(),
        new StateViewWithCommandsRule(),
        new StateChangeWithNoCommandsRule(),
        new StateChangeWithNoEventsRule(),
        new ExternalEventInNonTranslatorSliceRule(),
        new MissingEventSourceIdRule(),

        // Warning-level rules — likely architectural mistakes
        new StateViewWithNoReadModelsRule(),
        new EventSourceIdPropertyNotFoundRule(),

        // Advisory rules — improvements and best-practice guidance
        new DuplicateEventTypeNameRule(),
        new EventWithTooManyPropertiesRule(),
        new OrphanedEventRule(),
        new PrimitivePropertyTypeRule(),
        new CommandNamingConventionRule(),
        new EventNamingConventionRule(),
        new UnmappedReadModelPropertyRule(),
        new ReadModelWithoutKeyMappingRule()
    ];

    /// <inheritdoc/>
    public IReadOnlyList<EventModelRecommendation> Analyze(IEnumerable<Module> modules)
    {
        var moduleList = modules.ToList();

        return _rules
            .SelectMany(rule => rule.Evaluate(moduleList))
            .OrderByDescending(r => r.Severity)
            .ToList();
    }

    /// <inheritdoc/>
    public IReadOnlyList<EventModelRecommendation> Analyze(IEnumerable<Module> modules, IEnumerable<IEventModelRule> rules)
    {
        var moduleList = modules.ToList();

        return rules
            .SelectMany(rule => rule.Evaluate(moduleList))
            .OrderByDescending(r => r.Severity)
            .ToList();
    }
}
