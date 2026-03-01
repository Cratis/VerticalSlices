// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;

namespace Cratis.VerticalSlices;

/// <summary>
/// Defines an engine for working with vertical slices. The engine can generate code,
/// write it to an output target, and register artifacts with a Chronicle instance.
/// </summary>
public interface IVerticalSlicesEngine
{
    /// <summary>
    /// Generates code for all vertical slices in the given modules, writes them
    /// to the configured output, and registers artifacts with the configured Chronicle instance.
    /// </summary>
    /// <param name="modules">The modules containing features and vertical slices.</param>
    /// <param name="options">The code generation options that control how output is emitted. Defaults to per-file usings when null.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task Process(
        IEnumerable<Module> modules,
        CodeGenerationOptions? options = null,
        CancellationToken ct = default);

    /// <summary>
    /// Generates code for all vertical slices and returns the files in memory without writing them.
    /// Useful for preview in a UI.
    /// </summary>
    /// <param name="modules">The modules containing features and vertical slices.</param>
    /// <param name="options">The code generation options that control how output is emitted. Defaults to per-file usings when null.</param>
    /// <returns>The generated files.</returns>
    IEnumerable<RenderedArtifact> Preview(IEnumerable<Module> modules, CodeGenerationOptions? options = null);

    /// <summary>
    /// Generates code for a single vertical slice and returns the files in memory.
    /// Useful for previewing the output of one slice without processing the entire module tree.
    /// </summary>
    /// <param name="slice">The vertical slice to preview.</param>
    /// <param name="moduleName">The module name for namespace resolution.</param>
    /// <param name="featurePath">The path through the feature hierarchy for namespace resolution.</param>
    /// <param name="conceptScope">The resolved concept scope for type resolution. Defaults to <see cref="ConceptScope.Empty"/>.</param>
    /// <param name="options">The code generation options that control how output is emitted. Defaults to per-file usings when null.</param>
    /// <returns>The generated files for the single slice.</returns>
    IEnumerable<RenderedArtifact> PreviewSlice(
        VerticalSlice slice,
        string moduleName,
        FeaturePath featurePath,
        ConceptScope? conceptScope = null,
        CodeGenerationOptions? options = null);
}
