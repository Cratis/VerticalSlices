// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.Output;

/// <summary>
/// An <see cref="ICodeOutputResolver"/> that resolves to a single configured <see cref="ICodeOutput"/> instance.
/// When no output is configured, resolves to a <see cref="NoOpCodeOutput"/>.
/// </summary>
/// <param name="output">The code output to resolve to. When not provided, defaults to <see cref="NoOpCodeOutput"/>.</param>
public class CodeOutputResolver(ICodeOutput? output = null) : ICodeOutputResolver
{
    /// <inheritdoc/>
    public ICodeOutput Resolve() => output ?? new NoOpCodeOutput();
}
