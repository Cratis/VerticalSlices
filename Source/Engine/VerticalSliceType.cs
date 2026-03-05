// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices;

/// <summary>
/// The architectural pattern of a vertical slice.
/// </summary>
public enum VerticalSliceType
{
    /// <summary>
    /// Captures user intent through a screen, maps it to a command, and produces domain events.
    /// Flow: Screen → Command → EventType(s).
    /// </summary>
    StateChange = 0,

    /// <summary>
    /// Projects events into read models that are displayed on a screen.
    /// Flow: EventType(s) → ReadModel (projection + query) → Screen.
    /// </summary>
    StateView = 1,

    /// <summary>
    /// Reacts to events, maintains a task-list read model, and dispatches commands.
    /// Flow: EventType(s) → ReadModel → Command → EventType(s).
    /// </summary>
    Automation = 2,

    /// <summary>
    /// Translates external events into internal domain events.
    /// Flow: External EventType(s) → internal EventType(s).
    /// </summary>
    Translator = 3
}
