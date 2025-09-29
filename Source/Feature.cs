// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents a feature.
/// </summary>
/// <param name="Name">The name of the feature.</param>
/// <param name="Features">Any sub-features.</param>
/// <param name="VerticalSlices">The vertical slices in the feature.</param>
public record Feature(string Name, IEnumerable<Feature> Features, IEnumerable<VerticalSlice> VerticalSlices);
