// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.EventModelAdvisory;

/// <summary>
/// Defines the severity of an event model recommendation.
/// </summary>
public enum EventModelRecommendationSeverity
{
    /// <summary>
    /// Informational note about the event model that may not require action.
    /// </summary>
    Information = 0,

    /// <summary>
    /// A suggestion for improving the event model structure or naming.
    /// </summary>
    Suggestion = 1,

    /// <summary>
    /// A warning about a potential problem in the event model.
    /// </summary>
    Warning = 2,

    /// <summary>
    /// An error that will cause incorrect or broken code generation and must be fixed.
    /// </summary>
    Error = 3
}
