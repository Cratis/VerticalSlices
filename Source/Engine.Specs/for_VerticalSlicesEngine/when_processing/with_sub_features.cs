// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;
using Cratis.VerticalSlices.CodeGeneration.Renderers;

namespace Cratis.VerticalSlices.for_VerticalSlicesEngine.when_processing;

/// <summary>
/// Verifies that the engine recursively processes slices inside sub-features and writes their
/// generated files to the output.
/// </summary>
public class with_sub_features : given.all_dependencies
{
    VerticalSlicesEngine _engine;
    VerticalSlicesResult _result;
    IEnumerable<Module> _modules;
    RenderedArtifact _subFeatureFile;

    void Establish()
    {
        _engine = new VerticalSlicesEngine(_codeGenerator, _advisor, _logger, _loggerFactory);

        _subFeatureFile = new RenderedArtifact("Orders/Ordering/PlaceOrder/OrderPlaced.cs", "// generated");

        var command = new Command("PlaceOrder", "Places an order", [], "OrderId");
        var orderPlaced = new EventType("OrderPlaced", "An order was placed", []);
        var slice = new VerticalSlice("PlaceOrder", VerticalSliceType.StateChange, null, null, [command], [], [orderPlaced]);
        var subFeature = new Feature("PlaceOrder", [], [], [slice]);
        var parentFeature = new Feature("Ordering", [], [subFeature], []);
        _modules = [new Module("Orders", [], [parentFeature])];

        _codeGenerator
            .Generate(Arg.Any<VerticalSlice>(), Arg.Any<CodeGenerationContext>(), Arg.Any<ArtifactRenderSet>())
            .Returns([_subFeatureFile]);
    }

    async Task Because() => _result = await _engine.Process(_modules);

    [Fact] void should_call_code_generator_for_sub_feature_slice() =>
        _codeGenerator.Received(1).Generate(Arg.Any<VerticalSlice>(), Arg.Any<CodeGenerationContext>(), Arg.Any<ArtifactRenderSet>());

    [Fact] void should_include_sub_feature_file_in_result() => _result.Artifacts.ShouldContain(_subFeatureFile);
}
