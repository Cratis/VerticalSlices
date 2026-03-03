// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.Chronicle;

/// <summary>
/// Specifies which Chronicle registration backend to use.
/// </summary>
public enum ChronicleTarget
{
    /// <summary>No-op registration — artifacts are not pushed to Chronicle.</summary>
    NoOp = 0,

    /// <summary>Push artifacts to Chronicle over its HTTP REST API.</summary>
    Http = 1
}
