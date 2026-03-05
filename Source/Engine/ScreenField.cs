// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents a field on a screen.
/// </summary>
/// <param name="Name">The name of the field.</param>
/// <param name="Type">The data type of the field.</param>
/// <param name="FieldType">The UI field type.</param>
/// <param name="Label">The display label for the field.</param>
public record ScreenField(string Name, string Type, ScreenFieldType FieldType, string Label);
