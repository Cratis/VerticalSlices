// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents the different types of vertical slices.
/// </summary>
public static class VerticalSliceTypes
{
    /// <summary>
    /// Represents a state change vertical slice.
    /// </summary>
    public const string StateChange = "StateChange";

    /// <summary>
    /// Represents a read model vertical slice.
    /// </summary>
    public const string StateView = "StateView";

    /// <summary>
    /// Represents a process manager vertical slice.
    /// </summary>
    public const string Automation = "Automation";

    /// <summary>
    /// Represents a translator vertical slice.
    /// </summary>
    public const string Translator = "Translator";
}
