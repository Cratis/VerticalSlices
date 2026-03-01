// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;
using Cratis.VerticalSlices.CodeGeneration.Renderers;

namespace Cratis.VerticalSlices.for_VerticalSlicesEngine.when_previewing_slice;

/// <summary>
/// Specs for <see cref="VerticalSlicesEngine.PreviewSlice"/> with a single slice.
/// </summary>
public class with_a_single_slice : given.all_dependencies
{
    VerticalSlicesEngine _engine;
    VerticalSlice _slice;
    RenderedArtifact _generatedFile;
    IEnumerable<RenderedArtifact> _result;

    void Establish()
    {
        _engine = new VerticalSlicesEngine(_codeGenerator, _logger, _outputResolver, _chronicleResolver);
        _generatedFile = new RenderedArtifact("Orders/Placing/PlaceOrder.cs", "// generated");
        _slice = new VerticalSlice(
            "PlaceOrder",
            VerticalSliceType.StateChange,
            null,
            null,
            [new Command("PlaceOrder", "Places an order", [], "OrderId")],
            [],
            [new EventType("OrderPlaced", "An order was placed", [new Property("OrderId", "string")])]);

        _codeGenerator
            .Generate(Arg.Any<VerticalSlice>(), Arg.Any<CodeGenerationContext>(), Arg.Any<ArtifactRenderSet>())
            .Returns([_generatedFile]);
    }

    void Because() => _result = _engine.PreviewSlice(_slice, "Orders", new FeaturePath(["Placing"]));

    [Fact] void should_return_generated_files() => _result.ShouldContainOnly(_generatedFile);

    [Fact]
    void should_pass_correct_context_to_code_generator() =>
        _codeGenerator.Received(1).Generate(
            _slice,
            Arg.Is<CodeGenerationContext>(c => c.ModuleName == "Orders" && c.SliceName == "PlaceOrder"),
            Arg.Any<ArtifactRenderSet>());
}
