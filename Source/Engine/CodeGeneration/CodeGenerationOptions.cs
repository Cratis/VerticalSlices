// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration;

/// <summary>
/// Options that control how the code generator emits source files.
/// </summary>
public record CodeGenerationOptions
{
    /// <summary>
    /// Gets a value indicating whether to emit all using directives as <c>global using</c>
    /// declarations in a single generated file rather than as per-file using directives.
    /// When <see langword="false"/> (the default), each generated source file contains its own using directives.
    /// </summary>
    public bool UseGlobalUsings { get; init; }
}
