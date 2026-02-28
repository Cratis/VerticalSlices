// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.Integration.when_processing_a_module_with_multiple_slice_types;

/// <summary>
/// End-to-end scenario: a module that contains both a StateChange slice (event + command) and a
/// StateView slice (projection + observable query) that consumes the same event is fed to the
/// engine.  All generated files are written to disk and compiled in one <c>dotnet build</c>
/// invocation to verify that both slices produce valid, co-compilable C# output.
/// </summary>
public class with_state_change_and_state_view_slices : given.a_real_engine
{
    IEnumerable<Module> _modules;

    void Establish()
    {
        var sharedEvent = new EventType(
            "OrderPlaced",
            "An order was placed",
            [
                new Property("OrderId", "string"),
                new Property("CustomerId", "string"),
                new Property("Amount", "decimal")
            ]);

        var placeOrderCommand = new Command(
            "PlaceOrder",
            "Places a new customer order",
            [
                new Property("CustomerId", "string"),
                new Property("Amount", "decimal")
            ],
            "OrderId");

        var stateChangeSlice = new VerticalSlice(
            "PlaceOrder",
            VerticalSliceType.StateChange,
            null,
            null,
            [placeOrderCommand],
            [],
            [sharedEvent]);

        var orderIdMappings = new[] { new EventPropertyMapping("OrderPlaced", EventPropertyMappingKind.Set, "OrderId") };
        var customerIdMappings = new[] { new EventPropertyMapping("OrderPlaced", EventPropertyMappingKind.Set, "CustomerId") };
        var amountMappings = new[] { new EventPropertyMapping("OrderPlaced", EventPropertyMappingKind.Set, "Amount") };
        var countMappings = new[] { new EventPropertyMapping("OrderPlaced", EventPropertyMappingKind.Count) };

        var orderReadModel = new ReadModel(
            "Order",
            "Represents a placed order",
            [
                new ReadModelProperty("OrderId", "string", orderIdMappings),
                new ReadModelProperty("CustomerId", "string", customerIdMappings),
                new ReadModelProperty("Amount", "decimal", amountMappings),
                new ReadModelProperty("OrderCount", "int", countMappings)
            ]);

        var stateViewSlice = new VerticalSlice(
            "Orders",
            VerticalSliceType.StateView,
            null,
            null,
            [],
            [orderReadModel],
            [sharedEvent]);

        var feature = new Feature("Ordering", [], [], [stateChangeSlice, stateViewSlice]);
        _modules = [new Module("Sales", [], [feature])];
    }

    async Task Because()
    {
        await _engine.Process(_modules, _output);
        _generatedFiles = _engine.Preview(_modules);
        AddGlobalUsingsFromGeneratedFiles();
        _buildExitCode = await RunDotnet("build");
    }

    [Fact] void should_generate_event_type_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("OrderPlaced.cs")).ShouldBeTrue();

    [Fact] void should_generate_command_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("PlaceOrder.cs")).ShouldBeTrue();

    [Fact] void should_generate_projection_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("Order.cs")).ShouldBeTrue();

    [Fact] void should_generate_observable_query_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("AllOrders.cs")).ShouldBeTrue();

    [Fact] void should_compile_successfully() => _buildExitCode.ShouldEqual(0);
}
