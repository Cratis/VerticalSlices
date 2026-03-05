// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.Chronicle;
using Cratis.VerticalSlices.CodeGeneration;
using Cratis.VerticalSlices.CodeGeneration.Output;

namespace Cratis.VerticalSlices;

/// <summary>
/// Defines an engine for working with vertical slices. The engine can generate code,
/// write it to an output target, and register artifacts with a Chronicle instance.
/// </summary>
public interface IVerticalSlicesEngine
{
    /// <summary>
    /// Analyzes all vertical slices, generates code, writes output, and registers artifacts
    /// with the configured Chronicle instance.
    /// Returns a <see cref="VerticalSlicesResult"/> containing the full advisory findings,
    /// the generated artifacts, and the event/read-model descriptors collected for Chronicle.
    /// When <see cref="VerticalSlicesResult.HasErrors"/> is true, code generation was skipped
    /// and <see cref="VerticalSlicesResult.Artifacts"/> is empty.
    /// </summary>
    /// <param name="modules">The modules containing features and vertical slices.</param>
    /// <param name="options">The code generation options that control how output is emitted. Defaults to per-file usings when null.</param>
    /// <param name="outputOptions">The code output options controlling where generated files are written. Defaults to no-op when null.</param>
    /// <param name="chronicleOptions">The Chronicle options controlling artifact registration. Defaults to no-op when null.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A <see cref="Task{VerticalSlicesResult}"/> representing the asynchronous operation.</returns>
    Task<VerticalSlicesResult> Process(
        IEnumerable<Module> modules,
        CodeGenerationOptions? options = null,
        CodeOutputOptions? outputOptions = null,
        ChronicleOptions? chronicleOptions = null,
        CancellationToken ct = default);

    /// <summary>
    /// Analyzes and generates code for all vertical slices, returning the files and advisory
    /// findings in memory without writing them.
    /// When <see cref="VerticalSlicesResult.HasErrors"/> is true, <see cref="VerticalSlicesResult.Artifacts"/> is empty.
    /// </summary>
    /// <param name="modules">The modules containing features and vertical slices.</param>
    /// <param name="options">The code generation options that control how output is emitted. Defaults to per-file usings when null.</param>
    /// <returns>The recommendations and generated files.</returns>
    VerticalSlicesResult Preview(IEnumerable<Module> modules, CodeGenerationOptions? options = null);

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
