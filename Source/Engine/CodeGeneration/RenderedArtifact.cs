// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration;

/// <summary>
/// Represents a rendered artifact produced by a code generator or renderer.
/// Carries the logical path through the module/feature/slice hierarchy that outputs
/// use to determine where the artifact should be placed, and the generated content itself.
/// </summary>
/// <param name="ArtifactPath">The logical path through the hierarchy, e.g. <c>Orders/Placing/PlaceOrder.cs</c>.</param>
/// <param name="Content">The generated source content.</param>
public record RenderedArtifact(string ArtifactPath, string Content);
