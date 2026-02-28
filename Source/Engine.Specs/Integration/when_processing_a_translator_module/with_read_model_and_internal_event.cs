// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.Integration.when_processing_a_translator_module;

/// <summary>
/// End-to-end scenario: a Translator slice has both internal events (generated) and a read model.
/// The read model project the internal events and must compile together with them.
/// </summary>
public class with_read_model_and_internal_event : given.a_real_engine
{
    IEnumerable<Module> _modules;

    void Establish()
    {
        var externalEvent = new EventType(
            "ExternalShipmentArrived",
            "Shipment arrived from the external logistics system",
            [
                new Property("ShipmentRef", "string"),
                new Property("Warehouse", "string")
            ],
            EventKind.External);

        var internalEvent = new EventType(
            "ShipmentReceived",
            "Shipment ingested as internal domain event",
            [
                new Property("ShipmentId", "string"),
                new Property("WarehouseCode", "string")
            ],
            EventKind.Internal);

        var idMappings = new[]
        {
            new EventPropertyMapping("ShipmentReceived", EventPropertyMappingKind.Set, "ShipmentId")
        };

        var warehouseMappings = new[]
        {
            new EventPropertyMapping("ShipmentReceived", EventPropertyMappingKind.Set, "WarehouseCode")
        };

        var receivedShipment = new ReadModel(
            "ReceivedShipment",
            "A shipment that has been received and translated",
            [
                new ReadModelProperty("ShipmentId", "string", idMappings),
                new ReadModelProperty("WarehouseCode", "string", warehouseMappings)
            ]);

        var slice = new VerticalSlice(
            "ShipmentTranslation",
            VerticalSliceType.Translator,
            null,
            null,
            [],
            [receivedShipment],
            [externalEvent, internalEvent]);

        var feature = new Feature("Logistics", [], [], [slice]);
        _modules = [new Module("Warehouse", [], [feature])];
    }

    async Task Because()
    {
        await _engine.Process(_modules, _output);
        _generatedFiles = _engine.Preview(_modules);
        AddGlobalUsingsFromGeneratedFiles();
        _buildExitCode = await RunDotnet("build");
    }

    [Fact] void should_generate_internal_event_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("ShipmentReceived.cs")).ShouldBeTrue();

    [Fact] void should_not_generate_external_event_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("ExternalShipmentArrived.cs")).ShouldBeFalse();

    [Fact] void should_generate_read_model_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("ReceivedShipment.cs")).ShouldBeTrue();

    [Fact] void should_compile_successfully() => _buildExitCode.ShouldEqual(0);
}
