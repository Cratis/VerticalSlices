// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;
using Cratis.VerticalSlices.CodeGeneration.Renderers;

namespace Cratis.VerticalSlices.for_VerticalSliceCodeGenerator.when_generating;

/// <summary>
/// Verifies that the code generator correctly passes a context that includes
/// sub-features (nested path segments) through to the slice type generator unchanged.
/// </summary>
public class with_a_sub_feature_context : given.all_dependencies
{
    VerticalSlice _slice;
    CodeGenerationContext _subFeatureContext;
    GeneratedFile _expectedFile;
    IEnumerable<GeneratedFile> _result;

    void Establish()
    {
        _slice = new VerticalSlice("PlaceOrder", VerticalSliceType.StateChange, null, null, [], [], []);
        _subFeatureContext = new CodeGenerationContext("Orders", "Ordering", ["Placing"]);
        _expectedFile = new GeneratedFile("Orders/Ordering/Placing/PlaceOrder.cs", "// code");
        _stateChangeGenerator
            .Generate(Arg.Any<VerticalSlice>(), Arg.Any<CodeGenerationContext>(), Arg.Any<ArtifactRenderSet>())
            .Returns([_expectedFile]);
    }

    void Because() => _result = _generator.Generate(_slice, _subFeatureContext, _renderSet);

    [Fact] void should_delegate_to_the_slice_type_generator_with_the_sub_feature_context() =>
        _stateChangeGenerator.Received(1).Generate(_slice, _subFeatureContext, _renderSet);

    [Fact] void should_return_the_generated_file() => _result.ShouldContainOnly(_expectedFile);
}
