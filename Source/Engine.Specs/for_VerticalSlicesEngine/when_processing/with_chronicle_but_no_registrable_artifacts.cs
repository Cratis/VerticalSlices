// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.Chronicle;
using Cratis.VerticalSlices.CodeGeneration;
using Cratis.VerticalSlices.CodeGeneration.Descriptors;
using Cratis.VerticalSlices.CodeGeneration.Renderers;

namespace Cratis.VerticalSlices.for_VerticalSlicesEngine.when_processing;

/// <summary>
/// When chronicle is provided but the processed module produces no event descriptors
/// and no read model descriptors (e.g. Translator slice with only external events),
/// neither RegisterEventTypes nor RegisterProjections/RegisterReadModelTypes should
/// be called on the chronicle registration.
/// </summary>
public class with_chronicle_but_no_registrable_artifacts : given.all_dependencies
{
    IChronicleRegistration _chronicle;
    IEnumerable<Module> _modules;
    VerticalSlicesEngine _engine;

    void Establish()
    {
        _chronicle = Substitute.For<IChronicleRegistration>();
        _engine = new VerticalSlicesEngine(_codeGenerator, _logger, chronicle: _chronicle);

        // Translator slice with only an external event — produces no event descriptors
        // (external events are filtered out) and no read model descriptors.
        var externalEvent = new EventType("VendorInvoiceReceived", "External invoice", [], EventKind.External);
        var slice = new VerticalSlice(
            "ReceiveInvoice",
            VerticalSliceType.Translator,
            null,
            null,
            [],
            [],
            [externalEvent]);

        var feature = new Feature("Invoicing", [], [], [slice]);
        _modules = [new Module("Finance", [], [feature])];

        _codeGenerator
            .Generate(Arg.Any<VerticalSlice>(), Arg.Any<CodeGenerationContext>(), Arg.Any<ArtifactRenderSet>())
            .Returns([]);
    }

    async Task Because() => await _engine.Process(_modules);

    [Fact] void should_not_call_register_event_types() =>
        _chronicle.DidNotReceive().RegisterEventTypes(
            Arg.Any<IEnumerable<EventTypeDescriptor>>(),
            Arg.Any<CancellationToken>());

    [Fact] void should_not_call_register_projections() =>
        _chronicle.DidNotReceive().RegisterProjections(
            Arg.Any<IEnumerable<ReadModelDescriptor>>(),
            Arg.Any<CancellationToken>());

    [Fact] void should_not_call_register_read_model_types() =>
        _chronicle.DidNotReceive().RegisterReadModelTypes(
            Arg.Any<IEnumerable<ReadModelDescriptor>>(),
            Arg.Any<CancellationToken>());
}
