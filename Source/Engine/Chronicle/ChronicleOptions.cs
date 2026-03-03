// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.Chronicle;

/// <summary>
/// Configuration options that control which Chronicle registration backend is used.
/// </summary>
public class ChronicleOptions
{
    /// <summary>
    /// Gets or sets the Chronicle target. Defaults to <see cref="ChronicleTarget.NoOp"/>.
    /// </summary>
    public ChronicleTarget Target { get; set; } = ChronicleTarget.NoOp;

    /// <summary>
    /// Gets or sets the HTTP connection options used when <see cref="Target"/> is <see cref="ChronicleTarget.Http"/>.
    /// </summary>
    public ChronicleHttpOptions? HttpOptions { get; set; }
}
