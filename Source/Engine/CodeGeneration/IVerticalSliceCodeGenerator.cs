// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Renderers;

namespace Cratis.VerticalSlices.CodeGeneration;

/// <summary>
/// Defines a generator that produces all code files for a vertical slice.
/// </summary>
public interface IVerticalSliceCodeGenerator
{
    /// <summary>
    /// Generates all code files for a given vertical slice using the specified render set.
    /// </summary>
    /// <param name="slice">The vertical slice to generate code for.</param>
    /// <param name="context">The code generation context carrying namespace hierarchy.</param>
    /// <param name="renderSet">The artifact render set to use. Defaults to model-bound when null.</param>
    /// <returns>A collection of all generated files.</returns>
    IEnumerable<GeneratedFile> Generate(VerticalSlice slice, CodeGenerationContext context, ArtifactRenderSet? renderSet = null);
}
