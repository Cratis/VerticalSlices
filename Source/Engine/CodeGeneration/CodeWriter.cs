// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;

namespace Cratis.VerticalSlices.CodeGeneration;

/// <summary>
/// Static helpers for formatting C# declaration fragments such as record primary constructor parameter lists.
/// For full file construction, use <see cref="CSharpCodeBuilder"/> instead.
/// </summary>
public static class CodeWriter
{
    /// <summary>
    /// Formats a collection of properties as primary constructor parameters for a record.
    /// </summary>
    /// <param name="properties">The properties to format.</param>
    /// <returns>A formatted parameter list string.</returns>
    public static string FormatRecordParameters(IEnumerable<Property> properties)
    {
        var props = properties.ToList();
        if (props.Count == 0)
        {
            return string.Empty;
        }

        var builder = new StringBuilder();
        for (var i = 0; i < props.Count; i++)
        {
            if (i > 0)
            {
                builder.Append(", ");
            }

            builder.Append($"{props[i].Type} {props[i].Name}");
        }

        return builder.ToString();
    }

    /// <summary>
    /// Formats a collection of properties as primary constructor parameters for a record, each on its own line.
    /// </summary>
    /// <param name="properties">The properties to format.</param>
    /// <param name="indent">The indentation prefix for each parameter line.</param>
    /// <returns>A formatted multi-line parameter list string.</returns>
    public static string FormatRecordParametersMultiLine(IEnumerable<Property> properties, string indent = "    ")
    {
        var props = properties.ToList();
        if (props.Count == 0)
        {
            return string.Empty;
        }

        var builder = new StringBuilder();
        for (var i = 0; i < props.Count; i++)
        {
            builder.Append($"{indent}{props[i].Type} {props[i].Name}");
            if (i < props.Count - 1)
            {
                builder.Append(',');
            }

            builder.AppendLine();
        }

        return builder.ToString().TrimEnd();
    }
}
