// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.Integration.when_processing_a_module_with_multiple_slice_types;

/// <summary>
/// End-to-end scenario: a module with a StateChange slice (internal event + command)
/// and a Translator slice (external → internal event). Both slices' generated files
/// must compile together; the external event must not appear in the output.
/// </summary>
public class with_state_change_and_translator_slices : given.a_real_engine
{
    IEnumerable<Module> _modules;

    void Establish()
    {
        var domainEvent = new EventType(
            "ShipmentDispatched",
            "A shipment was dispatched",
            [
                new Property("ShipmentId", "string"),
                new Property("Carrier", "string")
            ]);

        var sendCommand = new Command(
            "DispatchShipment",
            "Dispatches a shipment",
            [
                new Property("Carrier", "string")
            ],
            "ShipmentId");

        var stateChangeSlice = new VerticalSlice(
            "DispatchShipment",
            VerticalSliceType.StateChange,
            null,
            null,
            [sendCommand],
            [],
            [domainEvent]);

        var externalEvent = new EventType(
            "ExternalCarrierUpdate",
            "An update received from the carrier",
            [new Property("TrackingRef", "string")],
            EventKind.External);

        var translatedEvent = new EventType(
            "CarrierUpdateIngested",
            "A carrier update was translated and ingested",
            [new Property("TrackingRef", "string")]);

        var translatorSlice = new VerticalSlice(
            "IngestCarrierUpdate",
            VerticalSliceType.Translator,
            null,
            null,
            [],
            [],
            [externalEvent, translatedEvent]);

        var feature = new Feature("Shipping", [], [], [stateChangeSlice, translatorSlice]);
        _modules = [new Module("Logistics", [], [feature])];
    }

    async Task Because()
    {
        await _engine.Process(_modules, _output);
        _generatedFiles = _engine.Preview(_modules);
        AddGlobalUsingsFromGeneratedFiles();
        _buildExitCode = await RunDotnet("build");
    }

    [Fact] void should_generate_state_change_event_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("ShipmentDispatched.cs")).ShouldBeTrue();

    [Fact] void should_generate_state_change_command_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("DispatchShipment.cs")).ShouldBeTrue();

    [Fact] void should_generate_translated_internal_event_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("CarrierUpdateIngested.cs")).ShouldBeTrue();

    [Fact] void should_not_generate_external_event_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("ExternalCarrierUpdate.cs")).ShouldBeFalse();

    [Fact] void should_compile_successfully() => _buildExitCode.ShouldEqual(0);
}
