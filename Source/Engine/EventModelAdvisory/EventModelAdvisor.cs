// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.DependencyInjection;
using Cratis.Types;

namespace Cratis.VerticalSlices.EventModelAdvisory;

/// <summary>
/// Analyzes an event model across all modules and produces recommendations
/// for improving the model structure, naming, coverage, and best practices.
/// </summary>
/// <param name="rules">The event model rules to evaluate against.</param>
[Singleton]
public class EventModelAdvisor(IInstancesOf<IEventModelRule> rules) : IEventModelAdvisor
{
    /// <inheritdoc/>
    public IReadOnlyList<EventModelRecommendation> Analyze(IEnumerable<Module> modules)
    {
        var moduleList = modules.ToList();

        return rules
            .SelectMany(rule => rule.Evaluate(moduleList))
            .OrderByDescending(r => r.Severity)
            .ToList();
    }

    /// <inheritdoc/>
    public IReadOnlyList<EventModelRecommendation> Analyze(IEnumerable<Module> modules, IEnumerable<IEventModelRule> specificRules)
    {
        var moduleList = modules.ToList();

        return specificRules
            .SelectMany(rule => rule.Evaluate(moduleList))
            .OrderByDescending(r => r.Severity)
            .ToList();
    }
}
