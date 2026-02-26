// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.Descriptors;

/// <summary>
/// Describes a single property in a command, including optional screen field metadata
/// when the property originates from a screen.
/// </summary>
/// <param name="Name">The property name.</param>
/// <param name="Type">The property type.</param>
/// <param name="FieldType">The UI field type, if this property is backed by a screen field.</param>
/// <param name="Label">The display label, if this property is backed by a screen field.</param>
public record CommandPropertyDescriptor(string Name, string Type, ScreenFieldType? FieldType, string? Label);
