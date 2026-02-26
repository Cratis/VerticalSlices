// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration;

/// <summary>
/// Represents a generated code file with its relative path and content.
/// </summary>
/// <param name="RelativePath">The relative file path where the file should be written.</param>
/// <param name="Content">The generated source code content.</param>
public record GeneratedFile(string RelativePath, string Content);
