// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents the type of a screen field.
/// </summary>
public enum ScreenFieldType
{
    /// <summary>
    /// A single-line text input.
    /// </summary>
    TextInput = 0,

    /// <summary>
    /// A multi-line text area.
    /// </summary>
    TextArea = 1,

    /// <summary>
    /// A numeric input.
    /// </summary>
    Number = 2,

    /// <summary>
    /// A date picker.
    /// </summary>
    Date = 3,

    /// <summary>
    /// A checkbox / boolean toggle.
    /// </summary>
    Checkbox = 4,

    /// <summary>
    /// A dropdown / select list.
    /// </summary>
    Dropdown = 5,

    /// <summary>
    /// A data table displaying rows.
    /// </summary>
    DataTable = 6
}
