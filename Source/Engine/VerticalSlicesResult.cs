// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;
using Cratis.VerticalSlices.EventModelAdvisory;

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents the outcome of an engine operation — carrying all advisory recommendations
/// alongside any artifacts that were produced.
/// When <see cref="HasErrors"/> is true, code generation was skipped and
/// <see cref="Artifacts"/> will be empty.
/// </summary>
/// <param name="Recommendations">All advisory findings ordered by descending severity.</param>
/// <param name="Artifacts">The generated artifact files. Empty when the operation was blocked by errors.</param>
public record VerticalSlicesResult(
    IReadOnlyList<EventModelRecommendation> Recommendations,
    IReadOnlyList<RenderedArtifact> Artifacts)
{
    /// <summary>
    /// Gets a value indicating whether any error-severity recommendations were found that blocked code generation.
    /// </summary>
    public bool HasErrors { get; } = Recommendations.Any(r => r.Severity == EventModelRecommendationSeverity.Error);
}
