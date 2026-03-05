// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json;

namespace Cratis.VerticalSlices.Chronicle;

/// <summary>
/// Generates JSON Schema strings from property descriptions.
/// Used when registering read model types with Chronicle's REST API.
/// </summary>
public static class JsonSchemaGenerator
{
    static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    /// <summary>
    /// Generates a JSON Schema object definition from a set of properties.
    /// </summary>
    /// <param name="properties">The properties to include in the schema.</param>
    /// <returns>A JSON Schema string.</returns>
    public static string Generate(IEnumerable<Property> properties)
    {
        var schemaProperties = new Dictionary<string, object>();

        foreach (var property in properties)
        {
            schemaProperties[ToCamelCase(property.Name)] = new Dictionary<string, string>
            {
                ["type"] = MapTypeToJsonSchemaType(property.Type)
            };
        }

        var schema = new Dictionary<string, object>
        {
            ["type"] = "object",
            ["properties"] = schemaProperties
        };

        return JsonSerializer.Serialize(schema, _jsonOptions);
    }

    /// <summary>
    /// Maps a C# type name to a JSON Schema type.
    /// </summary>
    /// <param name="typeName">The C# type name.</param>
    /// <returns>The corresponding JSON Schema type.</returns>
    static string MapTypeToJsonSchemaType(string typeName) =>
        typeName.ToUpperInvariant() switch
        {
            "STRING" => "string",
            "INT" or "INT32" or "INT64" or "LONG" or "SHORT" or "BYTE" => "integer",
            "FLOAT" or "DOUBLE" or "DECIMAL" or "SINGLE" => "number",
            "BOOL" or "BOOLEAN" => "boolean",
            "GUID" => "string",
            "DATETIME" or "DATETIMEOFFSET" or "DATEONLY" => "string",
            _ => "object"
        };

    /// <summary>
    /// Converts a PascalCase name to camelCase.
    /// </summary>
    /// <param name="name">The name to convert.</param>
    /// <returns>The camelCase version.</returns>
    static string ToCamelCase(string name) =>
        string.IsNullOrEmpty(name) ? name : char.ToLowerInvariant(name[0]) + name[1..];
}
