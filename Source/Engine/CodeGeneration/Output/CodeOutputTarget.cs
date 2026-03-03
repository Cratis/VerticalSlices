// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.Output;

/// <summary>
/// Specifies where generated code artifacts should be written.
/// </summary>
public enum CodeOutputTarget
{
    /// <summary>No output — generated files are discarded.</summary>
    NoOp = 0,

    /// <summary>Write generated files to the local file system.</summary>
    LocalFileSystem = 1,

    /// <summary>Keep generated files in memory; useful for testing.</summary>
    InMemory = 2
}
