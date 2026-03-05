// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Renderers.Controller;
using Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.for_ArtifactRenderSet.when_building_from_options;

/// <summary>
/// Specs for <see cref="ArtifactRenderSet.From"/> with controller read model endpoint style.
/// </summary>
public class with_controller_read_model_style : Specification
{
    ArtifactRenderSet _result;

    void Because() => _result = ArtifactRenderSet.From(new CodeGenerationOptions { ReadModelEndpointStyle = ReadModelEndpointStyle.Controller });

    [Fact] void should_use_model_bound_command_renderer() => _result.Command.ShouldBeOfExactType<ModelBoundCommandRenderer>();
    [Fact] void should_use_controller_read_model_renderer() => _result.ReadModel.ShouldBeOfExactType<ControllerReadModelRenderer>();
    [Fact] void should_use_model_bound_event_type_renderer() => _result.EventType.ShouldBeOfExactType<ModelBoundEventTypeRenderer>();
}
