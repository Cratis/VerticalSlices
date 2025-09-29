// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents a vertical slice.
/// </summary>
/// <param name="Name">The name of the vertical slice.</param>
/// <param name="VerticalSliceTypes">The type of the vertical slice.</param>
/// <param name="Commands">The commands in the vertical slice.</param>
/// <param name="Queries">The queries in the vertical slice.</param>
public record VerticalSlice(string Name, string VerticalSliceTypes, IEnumerable<Command> Commands, IEnumerable<Query> Queries);
