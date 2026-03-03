// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.Chronicle;
using Cratis.VerticalSlices.CodeGeneration;
using Cratis.VerticalSlices.CodeGeneration.Descriptors;
using Cratis.VerticalSlices.CodeGeneration.Output;
using Cratis.VerticalSlices.CodeGeneration.Renderers;

namespace Cratis.VerticalSlices.for_VerticalSlicesEngine.when_processing;

public class with_no_generated_files : given.all_dependencies
{
    IChronicleRegistration _chronicle;
    ICodeOutput _output;
    VerticalSlicesEngine _engine;
    IEnumerable<Module> _modules;

    void Establish()
    {
        _output = Substitute.For<ICodeOutput>();
        _chronicle = Substitute.For<IChronicleRegistration>();
        _engine = new VerticalSlicesEngine(_codeGenerator, _advisor, _logger, _loggerFactory);

        var slice = new VerticalSlice("EmptySlice", VerticalSliceType.StateView, null, null, [], [], []);
        _modules = [new Module("Mod", [], [new Feature("Feat", [], [], [slice])])];

        _codeGenerator
            .Generate(Arg.Any<VerticalSlice>(), Arg.Any<CodeGenerationContext>(), Arg.Any<ArtifactRenderSet>())
            .Returns([]);
    }

    async Task Because() => await _engine.Process(_modules, output: _output, chronicle: _chronicle);

    [Fact] void should_not_write_to_output() => _output.DidNotReceive().Write(Arg.Any<IEnumerable<RenderedArtifact>>(), Arg.Any<CancellationToken>());
    [Fact] void should_not_register_any_event_types() => _chronicle.DidNotReceive().RegisterEventTypes(Arg.Any<IEnumerable<EventTypeDescriptor>>(), Arg.Any<CancellationToken>());
    [Fact] void should_not_register_any_projections() => _chronicle.DidNotReceive().RegisterProjections(Arg.Any<IEnumerable<ReadModelDescriptor>>(), Arg.Any<CancellationToken>());
}
