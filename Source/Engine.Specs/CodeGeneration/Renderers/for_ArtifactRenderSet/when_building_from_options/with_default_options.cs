// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.for_ArtifactRenderSet.when_building_from_options;

/// <summary>
/// Specs for <see cref="ArtifactRenderSet.From"/> with default options.
/// </summary>
public class with_default_options : Specification
{
    ArtifactRenderSet _result;

    void Because() => _result = ArtifactRenderSet.From(new CodeGenerationOptions());

    [Fact] void should_use_model_bound_command_renderer() => _result.Command.ShouldBeOfExactType<ModelBoundCommandRenderer>();
    [Fact] void should_use_model_bound_read_model_renderer() => _result.ReadModel.ShouldBeOfExactType<ModelBoundReadModelRenderer>();
    [Fact] void should_use_model_bound_event_type_renderer() => _result.EventType.ShouldBeOfExactType<ModelBoundEventTypeRenderer>();
    [Fact] void should_use_model_bound_concept_renderer() => _result.Concept.ShouldBeOfExactType<ModelBoundConceptRenderer>();
}
