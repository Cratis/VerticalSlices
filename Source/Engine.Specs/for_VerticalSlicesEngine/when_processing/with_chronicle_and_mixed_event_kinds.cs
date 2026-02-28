// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.Chronicle;
using Cratis.VerticalSlices.CodeGeneration;
using Cratis.VerticalSlices.CodeGeneration.Descriptors;
using Cratis.VerticalSlices.CodeGeneration.Renderers;

namespace Cratis.VerticalSlices.for_VerticalSlicesEngine.when_processing;

/// <summary>
/// When a slice carries both external and internal events, only the internal events should be
/// registered with Chronicle. External events are structural artefacts only.
/// </summary>
public class with_chronicle_and_mixed_event_kinds : given.all_dependencies
{
    IChronicleRegistration _chronicle;
    IEnumerable<Module> _modules;
    VerticalSlicesEngine _engine;

    void Establish()
    {
        _chronicle = Substitute.For<IChronicleRegistration>();
        _engine = new VerticalSlicesEngine(_codeGenerator, _logger);

        var externalEvent = new EventType("ExternalPurchaseOrder", "External PO", [], EventKind.External);
        var internalEvent = new EventType("PurchaseOrderReceived", "PO received internally", [], EventKind.Internal);

        var slice = new VerticalSlice(
            "ReceivePO",
            VerticalSliceType.Translator,
            null,
            null,
            [],
            [],
            [externalEvent, internalEvent]);

        var feature = new Feature("Procurement", [], [], [slice]);
        _modules = [new Module("Purchasing", [], [feature])];

        _codeGenerator
            .Generate(Arg.Any<VerticalSlice>(), Arg.Any<CodeGenerationContext>(), Arg.Any<ArtifactRenderSet>())
            .Returns([]);
    }

    async Task Because() => await _engine.Process(_modules, chronicle: _chronicle);

    [Fact] void should_register_only_one_event_type() =>
        _chronicle.Received(1).RegisterEventTypes(
            Arg.Is<IEnumerable<EventTypeDescriptor>>(e => e.Count() == 1),
            Arg.Any<CancellationToken>());

    [Fact] void should_register_only_the_internal_event() =>
        _chronicle.Received(1).RegisterEventTypes(
            Arg.Is<IEnumerable<EventTypeDescriptor>>(e => e.First().Name == "PurchaseOrderReceived"),
            Arg.Any<CancellationToken>());

    [Fact] void should_not_register_external_event() =>
        _chronicle.DidNotReceive().RegisterEventTypes(
            Arg.Is<IEnumerable<EventTypeDescriptor>>(e => e.Any(d => d.Name == "ExternalPurchaseOrder")),
            Arg.Any<CancellationToken>());
}
