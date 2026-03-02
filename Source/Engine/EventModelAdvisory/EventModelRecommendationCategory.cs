// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.EventModelAdvisory;

/// <summary>
/// Defines the category of an event model recommendation.
/// </summary>
public enum EventModelRecommendationCategory
{
    /// <summary>
    /// Recommendations related to naming conventions for events, properties, or commands.
    /// </summary>
    Naming = 0,

    /// <summary>
    /// Recommendations related to the structural design of events.
    /// </summary>
    Structure = 1,

    /// <summary>
    /// Recommendations related to event coverage — events not consumed, properties not mapped, etc.
    /// </summary>
    Coverage = 2,

    /// <summary>
    /// Recommendations related to domain modeling best practices such as using concepts.
    /// </summary>
    BestPractice = 3
}
