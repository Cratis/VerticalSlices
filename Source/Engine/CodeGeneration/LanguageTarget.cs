// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration;

/// <summary>
/// Defines the target programming language for code generation.
/// </summary>
public enum LanguageTarget
{
    /// <summary>
    /// Generate C# (.cs) source files targeting .NET. This is the default.
    /// </summary>
    CSharp = 0,

    /// <summary>
    /// Generate TypeScript (.ts) source files.
    /// Reserved for future implementation.
    /// </summary>
    TypeScript = 1
}
