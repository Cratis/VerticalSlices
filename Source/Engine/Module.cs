// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents a module — the top-level grouping in the vertical slice hierarchy.
/// A module contains domain concepts shared across its features as well as one or more
/// <see cref="Feature"/> instances that hold the actual vertical slices.
/// The hierarchy is: Module → Feature (recursive) → VerticalSlice.
/// </summary>
/// <param name="Name">The module name.</param>
/// <param name="Concepts">Domain concepts defined at module scope.</param>
/// <param name="Features">The features that belong to this module.</param>
public record Module(string Name, IEnumerable<Concept> Concepts, IEnumerable<Feature> Features);
