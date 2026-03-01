// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;
using Cratis.VerticalSlices.CodeGeneration.Output;
using Cratis.VerticalSlices.CodeGeneration.Renderers;

namespace Cratis.VerticalSlices.for_VerticalSlicesEngine.when_processing;

/// <summary>
/// Verifies that the engine recursively processes slices inside sub-features and writes their
/// generated files to the output.
/// </summary>
public class with_sub_features : given.all_dependencies
{
    ICodeOutput _output;
    VerticalSlicesEngine _engine;
    IEnumerable<Module> _modules;
    RenderedArtifact _subFeatureFile;

    void Establish()
    {
        _output = Substitute.For<ICodeOutput>();
        _outputResolver.Resolve().Returns(_output);
        _engine = new VerticalSlicesEngine(_codeGenerator, _logger, _outputResolver, _chronicleResolver);

        _subFeatureFile = new RenderedArtifact("Orders/Ordering/PlaceOrder/OrderPlaced.cs", "// generated");

        var slice = new VerticalSlice("PlaceOrder", VerticalSliceType.StateChange, null, null, [], [], []);
        var subFeature = new Feature("PlaceOrder", [], [], [slice]);
        var parentFeature = new Feature("Ordering", [], [subFeature], []);
        _modules = [new Module("Orders", [], [parentFeature])];

        _codeGenerator
            .Generate(Arg.Any<VerticalSlice>(), Arg.Any<CodeGenerationContext>(), Arg.Any<ArtifactRenderSet>())
            .Returns([_subFeatureFile]);
    }

    async Task Because() => await _engine.Process(_modules);

    [Fact] void should_call_code_generator_for_sub_feature_slice() =>
        _codeGenerator.Received(1).Generate(Arg.Any<VerticalSlice>(), Arg.Any<CodeGenerationContext>(), Arg.Any<ArtifactRenderSet>());

    [Fact] void should_write_sub_feature_files_to_output() =>
        _output.Received(1).Write(Arg.Is<IEnumerable<RenderedArtifact>>(files =>
            files.Contains(_subFeatureFile)), Arg.Any<CancellationToken>());
}
