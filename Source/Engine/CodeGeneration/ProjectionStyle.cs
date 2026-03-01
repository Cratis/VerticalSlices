// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration;

/// <summary>
/// Defines how projections are rendered for read models.
/// </summary>
public enum ProjectionStyle
{
    /// <summary>
    /// Uses Chronicle model-bound attributes (<c>[ReadModel]</c>, <c>[FromEvent]</c>, <c>[SetFrom]</c>, etc.)
    /// on the read model record itself. Suitable for simple, attribute-friendly projections.
    /// </summary>
    ModelBound = 0,

    /// <summary>
    /// Produces a separate <c>IProjectionFor&lt;T&gt;</c> class that uses the fluent projection builder API.
    /// Suitable for complex projections with joins, children, or custom logic.
    /// </summary>
    Declarative = 1,

    /// <summary>
    /// Produces a separate <c>IReducerOf&lt;T&gt;</c> class that applies events through a reducer pattern.
    /// Suitable for stateful aggregation scenarios.
    /// </summary>
    Reducer = 2
}
