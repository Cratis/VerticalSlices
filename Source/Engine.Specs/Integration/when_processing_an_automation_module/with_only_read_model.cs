// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.Integration.when_processing_an_automation_module;

/// <summary>
/// End-to-end scenario: an Automation slice that contains only a read model (no
/// commands). The generated read model projection file must compile correctly.
/// </summary>
public class with_only_read_model : given.a_real_engine
{
    IEnumerable<Module> _modules;

    void Establish()
    {
        var orderEvent = new EventType(
            "OrderFulfilled",
            "An order was fulfilled",
            [new Property("OrderId", "string"), new Property("FulfilledAt", "string")]);

        var orderIdMappings = new[]
        {
            new EventPropertyMapping("OrderFulfilled", EventPropertyMappingKind.Set, "OrderId")
        };

        var fulfilledAtMappings = new[]
        {
            new EventPropertyMapping("OrderFulfilled", EventPropertyMappingKind.Set, "FulfilledAt")
        };

        var fulfilledOrderReadModel = new ReadModel(
            "FulfilledOrder",
            "A fulfilled order summary",
            [
                new ReadModelProperty("OrderId", "string", orderIdMappings),
                new ReadModelProperty("FulfilledAt", "string", fulfilledAtMappings)
            ]);

        var slice = new VerticalSlice(
            "FulfilledOrders",
            VerticalSliceType.Automation,
            null,
            null,
            [],
            [fulfilledOrderReadModel],
            [orderEvent]);

        var feature = new Feature("Fulfillment", [], [], [slice]);
        _modules = [new Module("Orders", [], [feature])];
    }

    async Task Because()
    {
        await _engine.Process(_modules, _output);
        _generatedFiles = _engine.Preview(_modules);
        AddGlobalUsingsFromGeneratedFiles();
        _buildExitCode = await RunDotnet("build");
    }

    [Fact] void should_generate_read_model_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("FulfilledOrder.cs")).ShouldBeTrue();

    [Fact] void should_not_generate_command_file() =>
        _generatedFiles.All(f => !f.RelativePath.Contains("Command")).ShouldBeTrue();

    [Fact] void should_compile_successfully() => _buildExitCode.ShouldEqual(0);
}
