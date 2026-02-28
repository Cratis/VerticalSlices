// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.Renderers;

/// <summary>
/// Defines a renderer that converts an artifact descriptor into generated code files.
/// Different implementations produce different code variations (e.g., model-bound, controller-based).
/// </summary>
/// <typeparam name="TDescriptor">The type of artifact descriptor this renderer accepts.</typeparam>
public interface IArtifactRenderer<in TDescriptor>
{
    /// <summary>
    /// Renders the given descriptor into one or more generated code files.
    /// </summary>
    /// <param name="descriptor">The artifact descriptor to render.</param>
    /// <param name="context">The code generation context carrying namespace hierarchy.</param>
    /// <returns>A collection of generated files.</returns>
    IEnumerable<RenderedArtifact> Render(TDescriptor descriptor, CodeGenerationContext context);
}
