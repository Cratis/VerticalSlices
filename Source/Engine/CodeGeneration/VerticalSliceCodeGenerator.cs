// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Renderers;
using Cratis.VerticalSlices.CodeGeneration.SliceTypes;
using Microsoft.Extensions.Logging;

namespace Cratis.VerticalSlices.CodeGeneration;

/// <summary>
/// Orchestrates code generation by dispatching to the correct slice type generator.
/// </summary>
/// <param name="sliceTypeGenerators">The slice type specific code generators.</param>
/// <param name="logger">The logger.</param>
public partial class VerticalSliceCodeGenerator(
    IEnumerable<ISliceTypeCodeGenerator> sliceTypeGenerators,
    ILogger<VerticalSliceCodeGenerator> logger) : IVerticalSliceCodeGenerator
{
    readonly Dictionary<VerticalSliceType, ISliceTypeCodeGenerator> _generatorsBySliceType =
        sliceTypeGenerators.ToDictionary(g => g.SliceType);

    /// <inheritdoc/>
    public IEnumerable<RenderedArtifact> Generate(VerticalSlice slice, CodeGenerationContext context, ArtifactRenderSet? renderSet = null)
    {
        renderSet ??= ArtifactRenderSet.ModelBound;

        if (!_generatorsBySliceType.TryGetValue(slice.SliceType, out var generator))
        {
            LogUnsupportedSliceType(slice.SliceType, slice.Name);

            return [];
        }

        LogGeneratingSlice(slice.Name, slice.SliceType);

        return generator.Generate(slice, context, renderSet);
    }

    [LoggerMessage(LogLevel.Warning, "Unsupported slice type '{SliceType}' for slice '{SliceName}', skipping code generation")]
    partial void LogUnsupportedSliceType(VerticalSliceType sliceType, string sliceName);

    [LoggerMessage(LogLevel.Debug, "Generating code for slice '{SliceName}' of type '{SliceType}'")]
    partial void LogGeneratingSlice(string sliceName, VerticalSliceType sliceType);
}
