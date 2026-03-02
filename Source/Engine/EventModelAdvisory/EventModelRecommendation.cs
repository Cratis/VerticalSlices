// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.EventModelAdvisory;

/// <summary>
/// Represents a single recommendation about an event model.
/// </summary>
/// <param name="Severity">How important the recommendation is.</param>
/// <param name="Category">The category of the recommendation.</param>
/// <param name="ModuleName">The name of the module the recommendation originates from. Empty when not module-specific.</param>
/// <param name="FeaturePath">The path through the feature hierarchy to the slice. Empty when the recommendation is cross-slice or module-level.</param>
/// <param name="SliceName">The name of the slice the recommendation applies to. Empty when the recommendation is cross-slice.</param>
/// <param name="ArtifactName">The name of the artifact (event type, read model, command) the recommendation targets. Empty when the recommendation is not artifact-specific.</param>
/// <param name="Message">A human-readable description of the recommendation.</param>
/// <param name="Suggestion">An optional actionable suggestion for how to address the recommendation.</param>
public record EventModelRecommendation(
    EventModelRecommendationSeverity Severity,
    EventModelRecommendationCategory Category,
    string ModuleName,
    FeaturePath FeaturePath,
    string SliceName,
    string ArtifactName,
    string Message,
    string? Suggestion = null);
