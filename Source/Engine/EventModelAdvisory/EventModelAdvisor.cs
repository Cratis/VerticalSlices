// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory.Rules;

namespace Cratis.VerticalSlices.EventModelAdvisory;

/// <summary>
/// Analyzes an event model across all modules and produces recommendations
/// for improving the model structure, naming, coverage, and best practices.
/// </summary>
public static class EventModelAdvisor
{
    static readonly IEventModelRule[] _rules =
    [

        // Error-level rules — must be fixed before code generation can succeed
        new DuplicatePropertyNameRule(),
        new DuplicateArtifactNameInSliceRule(),
        new ProducedEventNotDefinedRule(),
        new MissingSourcePropertyInMappingRule(),
        new EventSourceIdPropertyNotFoundRule(),
        new UnresolvedEventReferenceRule(),
        new EventPropertyMappingSourcePropertyNotFoundRule(),
        new StateViewWithCommandsRule(),

        // Warning-level rules — likely architectural mistakes
        new StateViewWithNoReadModelsRule(),

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

    /// <summary>
    /// Analyzes the event model and returns all recommendations from all rules.
    /// </summary>
    /// <param name="modules">The modules containing the event model to analyze.</param>
    /// <returns>All recommendations produced by the advisory rules, ordered by severity.</returns>
    public static IReadOnlyList<EventModelRecommendation> Analyze(IEnumerable<Module> modules)
    {
        var moduleList = modules.ToList();

        return _rules
            .SelectMany(rule => rule.Evaluate(moduleList))
            .OrderByDescending(r => r.Severity)
            .ToList();
    }

    /// <summary>
    /// Analyzes the event model using only the specified rules.
    /// </summary>
    /// <param name="modules">The modules containing the event model to analyze.</param>
    /// <param name="rules">The specific rules to evaluate.</param>
    /// <returns>All recommendations produced by the specified rules, ordered by severity.</returns>
    public static IReadOnlyList<EventModelRecommendation> Analyze(IEnumerable<Module> modules, IEnumerable<IEventModelRule> rules)
    {
        var moduleList = modules.ToList();

        return rules
            .SelectMany(rule => rule.Evaluate(moduleList))
            .OrderByDescending(r => r.Severity)
            .ToList();
    }
}
