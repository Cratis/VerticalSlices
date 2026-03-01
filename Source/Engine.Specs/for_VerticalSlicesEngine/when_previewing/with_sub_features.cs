// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;
using Cratis.VerticalSlices.CodeGeneration.Renderers;

namespace Cratis.VerticalSlices.for_VerticalSlicesEngine.when_previewing;

/// <summary>
/// Verifies that the engine recursively processes slices inside sub-features.
/// </summary>
public class with_sub_features : given.all_dependencies
{
    VerticalSlicesEngine _engine;
    IEnumerable<Module> _modules;
    IEnumerable<RenderedArtifact> _result;
    RenderedArtifact _subFeatureFile;

    void Establish()
    {
        _engine = new VerticalSlicesEngine(_codeGenerator, _logger, _outputResolver, _chronicleResolver);

        _subFeatureFile = new RenderedArtifact("Orders/Ordering/PlaceOrder/PlaceOrderCommand.cs", "// generated");

        var slice = new VerticalSlice("PlaceOrder", VerticalSliceType.StateChange, null, null, [], [], []);
        var subFeature = new Feature("PlaceOrder", [], [], [slice]);
        var parentFeature = new Feature("Ordering", [], [subFeature], []);
        _modules = [new Module("Orders", [], [parentFeature])];

        _codeGenerator
            .Generate(Arg.Any<VerticalSlice>(), Arg.Any<CodeGenerationContext>(), Arg.Any<ArtifactRenderSet>())
            .Returns([_subFeatureFile]);
    }

    void Because() => _result = _engine.Preview(_modules);

    [Fact] void should_include_file_from_sub_feature_slice() => _result.ShouldContainOnly(_subFeatureFile);
}
