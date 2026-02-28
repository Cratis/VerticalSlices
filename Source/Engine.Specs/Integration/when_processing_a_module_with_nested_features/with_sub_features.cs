// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.Integration.when_processing_a_module_with_nested_features;

/// <summary>
/// End-to-end scenario: a module containing a feature with a nested sub-feature  is fed to
/// the engine. The generated C# files are written to disk and then compiled with
/// <c>dotnet build</c> to verify the generated code is valid.
/// </summary>
public class with_sub_features : given.a_real_engine
{
    IEnumerable<Module> _modules;

    void Establish()
    {
        var orderPlacedEvent = new EventType(
            "OrderPlaced",
            "An order was placed",
            [
                new Property("OrderId", "string"),
                new Property("Amount", "decimal")
            ]);

        var placeOrderCommand = new Command(
            "PlaceOrder",
            "Places a new order",
            [
                new Property("Amount", "decimal")
            ],
            "OrderId");

        // StateChange slice lives inside the "Placing" sub-feature
        var placeOrderSlice = new VerticalSlice(
            "PlaceOrder",
            VerticalSliceType.StateChange,
            null,
            null,
            [placeOrderCommand],
            [],
            [orderPlacedEvent]);

        var orderCancelledEvent = new EventType(
            "OrderCancelled",
            "An order was cancelled",
            [new Property("OrderId", "string")]);

        var cancelOrderCommand = new Command(
            "CancelOrder",
            "Cancels an existing order",
            [],
            "OrderId");

        var cancelOrderSlice = new VerticalSlice(
            "CancelOrder",
            VerticalSliceType.StateChange,
            null,
            null,
            [cancelOrderCommand],
            [],
            [orderCancelledEvent]);

        // Sub-features nested inside the parent "Orders" feature
        var placingSubFeature = new Feature("Placing", [], [], [placeOrderSlice]);
        var cancellingSubFeature = new Feature("Cancelling", [], [], [cancelOrderSlice]);
        var ordersFeature = new Feature("Orders", [], [placingSubFeature, cancellingSubFeature], []);
        _modules = [new Module("OrderManagement", [], [ordersFeature])];
    }

    async Task Because()
    {
        await _engine.Process(_modules, _output);
        _generatedFiles = _engine.Preview(_modules);
        AddGlobalUsingsFromGeneratedFiles();
        _buildExitCode = await RunDotnet("build");
    }

    [Fact] void should_generate_event_from_first_sub_feature() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("OrderPlaced.cs")).ShouldBeTrue();

    [Fact] void should_generate_command_from_first_sub_feature() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("PlaceOrder.cs")).ShouldBeTrue();

    [Fact] void should_generate_event_from_second_sub_feature() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("OrderCancelled.cs")).ShouldBeTrue();

    [Fact] void should_generate_command_from_second_sub_feature() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("CancelOrder.cs")).ShouldBeTrue();

    [Fact] void should_compile_successfully() => _buildExitCode.ShouldEqual(0);
}
