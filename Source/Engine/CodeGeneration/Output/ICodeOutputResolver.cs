// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.Output;

/// <summary>
/// Defines a resolver for determining which <see cref="ICodeOutput"/> to use
/// based on configuration and consumer requirements.
/// </summary>
public interface ICodeOutputResolver
{
    /// <summary>
    /// Resolves the code output strategy to use for writing generated artifacts.
    /// When no real output is configured, returns a <see cref="NoOpCodeOutput"/>.
    /// </summary>
    /// <returns>The resolved <see cref="ICodeOutput"/>.</returns>
    ICodeOutput Resolve();
}
