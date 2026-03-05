// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.for_ArtifactRenderSet.when_accessing_model_bound;

public class the_render_set : Specification
{
    ArtifactRenderSet _result;

    void Because() => _result = ArtifactRenderSet.ModelBound;

    [Fact] void should_not_be_null() => _result.ShouldNotBeNull();
    [Fact] void should_have_event_type_renderer() => _result.EventType.ShouldNotBeNull();
    [Fact] void should_have_command_renderer() => _result.Command.ShouldNotBeNull();
    [Fact] void should_have_read_model_renderer() => _result.ReadModel.ShouldNotBeNull();
    [Fact] void should_have_concept_renderer() => _result.Concept.ShouldNotBeNull();
    [Fact] void should_use_model_bound_event_type_renderer() => _result.EventType.ShouldBeOfExactType<ModelBound.ModelBoundEventTypeRenderer>();
    [Fact] void should_use_model_bound_command_renderer() => _result.Command.ShouldBeOfExactType<ModelBound.ModelBoundCommandRenderer>();
    [Fact] void should_use_model_bound_read_model_renderer() => _result.ReadModel.ShouldBeOfExactType<ModelBound.ModelBoundReadModelRenderer>();
    [Fact] void should_use_model_bound_concept_renderer() => _result.Concept.ShouldBeOfExactType<ModelBound.ModelBoundConceptRenderer>();
}
