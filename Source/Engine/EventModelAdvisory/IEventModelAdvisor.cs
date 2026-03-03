// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.EventModelAdvisory;

/// <summary>
/// Defines a service that analyzes an event model and produces recommendations
/// for improving the model structure, naming, coverage, and best practices.
/// </summary>
public interface IEventModelAdvisor
{
    /// <summary>
    /// Analyzes the event model and returns all recommendations from all configured rules.
    /// </summary>
    /// <param name="modules">The modules containing the event model to analyze.</param>
    /// <returns>All recommendations produced by the advisory rules, ordered by severity.</returns>
    IReadOnlyList<EventModelRecommendation> Analyze(IEnumerable<Module> modules);

    /// <summary>
    /// Analyzes the event model using only the specified rules.
    /// </summary>
    /// <param name="modules">The modules containing the event model to analyze.</param>
    /// <param name="specificRules">The specific rules to evaluate.</param>
    /// <returns>All recommendations produced by the specified rules, ordered by severity.</returns>
    IReadOnlyList<EventModelRecommendation> Analyze(IEnumerable<Module> modules, IEnumerable<IEventModelRule> specificRules);
}
