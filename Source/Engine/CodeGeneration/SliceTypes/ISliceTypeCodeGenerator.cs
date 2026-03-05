// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Renderers;

namespace Cratis.VerticalSlices.CodeGeneration.SliceTypes;

/// <summary>
/// Defines a code generator for a specific vertical slice type.
/// </summary>
public interface ISliceTypeCodeGenerator
{
    /// <summary>
    /// Gets the slice type this generator handles.
    /// </summary>
    VerticalSliceType SliceType { get; }

    /// <summary>
    /// Generates code files for a vertical slice of this type using the provided render set.
    /// </summary>
    /// <param name="slice">The vertical slice to generate code for.</param>
    /// <param name="context">The code generation context carrying namespace hierarchy.</param>
    /// <param name="renderSet">The artifact render set defining how each artifact is rendered to code.</param>
    /// <returns>A collection of generated files.</returns>
    IEnumerable<RenderedArtifact> Generate(VerticalSlice slice, CodeGenerationContext context, ArtifactRenderSet renderSet);
}
