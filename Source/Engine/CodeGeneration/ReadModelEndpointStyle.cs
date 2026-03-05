// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration;

/// <summary>
/// Defines how read model query endpoints are rendered.
/// </summary>
public enum ReadModelEndpointStyle
{
    /// <summary>
    /// Uses the Chronicle <c>[ReadModel]</c> attribute which provides both projection and query endpoint
    /// through a single model-bound record. This is the default approach.
    /// </summary>
    ModelBound = 0,

    /// <summary>
    /// The read model has a separate <c>[ApiController]</c> that queries the MongoDB collection directly.
    /// Used when the projection and query endpoint need to be independent, or when controller
    /// flexibility is required (e.g., custom filtering, pagination).
    /// </summary>
    Controller = 1
}
