// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.Output;

/// <summary>
/// Configuration options that control where generated code artifacts are written.
/// </summary>
public class CodeOutputOptions
{
    /// <summary>
    /// Gets or sets the output target. Defaults to <see cref="CodeOutputTarget.NoOp"/>.
    /// </summary>
    public CodeOutputTarget Target { get; set; } = CodeOutputTarget.NoOp;

    /// <summary>
    /// Gets or sets the root directory used when <see cref="Target"/> is <see cref="CodeOutputTarget.LocalFileSystem"/>.
    /// </summary>
    public string OutputRoot { get; set; } = string.Empty;
}
