// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.Descriptors;

/// <summary>
/// Describes a single property in a read model, including its key status, event mappings,
/// and optional screen field metadata.
/// </summary>
/// <param name="Name">The property name.</param>
/// <param name="Type">The property type.</param>
/// <param name="IsKey">Whether this property is the key/identity of the read model.</param>
/// <param name="Mappings">The property mappings describing how events populate this property.</param>
/// <param name="FieldType">The UI field type, if this property is displayed on a screen.</param>
/// <param name="Label">The display label, if this property is displayed on a screen.</param>
public record ReadModelPropertyDescriptor(
    string Name,
    string Type,
    bool IsKey,
    IEnumerable<PropertyMapping> Mappings,
    ScreenFieldType? FieldType = null,
    string? Label = null);
