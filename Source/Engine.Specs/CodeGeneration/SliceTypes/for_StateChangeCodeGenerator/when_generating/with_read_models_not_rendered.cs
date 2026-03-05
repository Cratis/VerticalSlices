// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.SliceTypes.for_StateChangeCodeGenerator.when_generating;

/// <summary>
/// Verifies that StateChangeCodeGenerator ignores any ReadModels on the slice — it is not
/// responsible for read model generation and must not invoke the read model renderer.
/// </summary>
public class with_read_models_not_rendered : given.a_slice_type_code_generator
{
    StateChangeCodeGenerator _generator;
    VerticalSlice _slice;
    IEnumerable<RenderedArtifact> _result;

    void Establish()
    {
        _generator = new StateChangeCodeGenerator();
        var readModel = new ReadModel("OrderList", "Orders", []);
        _slice = new VerticalSlice("PlaceOrder", VerticalSliceType.StateChange, null, null, [], [readModel], []);
    }

    void Because() => _result = _generator.Generate(_slice, _context, _renderSet);

    [Fact] void should_return_no_files() => _result.ShouldBeEmpty();
    [Fact] void should_not_call_read_model_renderer() => _readModelRenderer.DidNotReceiveWithAnyArgs().Render(default!, default!);
}
