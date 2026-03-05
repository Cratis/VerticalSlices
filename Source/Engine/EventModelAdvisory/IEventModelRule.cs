// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.EventModelAdvisory;

/// <summary>
/// Defines a rule that evaluates the event model and produces recommendations.
/// </summary>
public interface IEventModelRule
{
    /// <summary>
    /// Evaluates the event model across all modules and returns any recommendations.
    /// </summary>
    /// <param name="modules">The modules containing the event model to evaluate.</param>
    /// <returns>The recommendations produced by this rule.</returns>
    IEnumerable<EventModelRecommendation> Evaluate(IEnumerable<Module> modules);
}
