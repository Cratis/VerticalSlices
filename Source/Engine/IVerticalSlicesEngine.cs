// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices;

/// <summary>
/// Defines an engine for setting up vertical slices with Chronicle.
/// </summary>
public interface IVerticalSlicesEngine
{
    /// <summary>
    /// Sets up the vertical slices structure with Chronicle.
    /// </summary>
    /// <param name="features">The features to set up.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task Setup(IEnumerable<Feature> features);
}
