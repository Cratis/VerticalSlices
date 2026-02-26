// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents a screen associated with a vertical slice.
/// </summary>
/// <param name="Name">The name of the screen.</param>
/// <param name="Description">A description of what the screen does.</param>
/// <param name="Fields">The fields displayed or captured by the screen.</param>
public record Screen(string Name, string Description, IEnumerable<ScreenField> Fields);
