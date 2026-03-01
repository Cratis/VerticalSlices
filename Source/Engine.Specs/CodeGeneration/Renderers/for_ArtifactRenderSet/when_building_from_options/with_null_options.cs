// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Renderers.Controller;
using Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.for_ArtifactRenderSet.when_building_from_options;

/// <summary>
/// Specs for <see cref="ArtifactRenderSet.From"/> with <see langword="null"/> options.
/// </summary>
public class with_null_options : Specification
{
    ArtifactRenderSet _result;

    void Because() => _result = ArtifactRenderSet.From(null);

    [Fact] void should_return_model_bound_set() => _result.ShouldEqual(ArtifactRenderSet.ModelBound);
}
