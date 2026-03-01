// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration;

/// <summary>
/// Defines how commands are rendered to code.
/// </summary>
public enum CommandStyle
{
    /// <summary>
    /// The command is a <c>[Command]</c> record with a <c>Handle()</c> method that the framework discovers by convention.
    /// This is the default Arc model-bound pattern.
    /// </summary>
    ModelBound = 0,

    /// <summary>
    /// The command is a plain record and a separate ASP.NET Core controller handles it.
    /// Produces a <c>[ApiController]</c> with a <c>POST</c> endpoint.
    /// </summary>
    Controller = 1
}
