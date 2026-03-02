// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.DependencyInjection;

namespace Cratis.VerticalSlices.CodeGeneration.Output;

/// <summary>
/// An <see cref="ICodeOutputResolver"/> that resolves to a single configured <see cref="ICodeOutput"/> instance.
/// </summary>
/// <param name="output">The code output to resolve to.</param>
[Singleton]
public class CodeOutputResolver(ICodeOutput output) : ICodeOutputResolver
{
    /// <inheritdoc/>
    public ICodeOutput Resolve() => output;
}
