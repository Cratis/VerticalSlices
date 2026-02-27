// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents a vertical slice.
/// </summary>
/// <param name="Name">The name of the vertical slice.</param>
/// <param name="SliceType">The architectural pattern of this vertical slice.</param>
/// <param name="Screen">The screen associated with this slice.</param>
/// <param name="Commands">The commands in the vertical slice.</param>
/// <param name="ReadModels">The read models in the vertical slice.</param>
/// <param name="Events">The event types produced by this slice (e.g. events raised by commands in a StateChange slice).</param>
public record VerticalSlice(
    string Name,
    VerticalSliceType SliceType,
    Screen? Screen,
    IEnumerable<Command> Commands,
    IEnumerable<ReadModel> ReadModels,
    IEnumerable<EventType> Events);
