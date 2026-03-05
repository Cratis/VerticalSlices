// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;
using Cratis.VerticalSlices.CodeGeneration.Renderers;

namespace Cratis.VerticalSlices.for_VerticalSlicesEngine.when_previewing_slice;

/// <summary>
/// Verifies that <see cref="CodeGenerationOptions"/> passed to <see cref="VerticalSlicesEngine.PreviewSlice"/>
/// are forwarded to the code generator via the <see cref="CodeGenerationContext"/>.
/// </summary>
public class with_custom_options : given.all_dependencies
{
    VerticalSlicesEngine _engine;
    VerticalSlice _slice;
    CodeGenerationOptions _options;

    void Establish()
    {
        _engine = new VerticalSlicesEngine(_codeGenerator, _advisor, _logger, _loggerFactory);
        _slice = new VerticalSlice("PlaceOrder", VerticalSliceType.StateChange, null, null, [], [], []);
        _options = new CodeGenerationOptions { SingleFilePerSlice = false };

        _codeGenerator
            .Generate(Arg.Any<VerticalSlice>(), Arg.Any<CodeGenerationContext>(), Arg.Any<ArtifactRenderSet>())
            .Returns([]);
    }

    void Because() => _engine.PreviewSlice(_slice, "Orders", new FeaturePath(["Ordering"]), options: _options);

    [Fact]
    void should_pass_options_to_code_generator() =>
        _codeGenerator.Received(1).Generate(
            _slice,
            Arg.Is<CodeGenerationContext>(c => !c.Options.SingleFilePerSlice),
            Arg.Any<ArtifactRenderSet>());
}
