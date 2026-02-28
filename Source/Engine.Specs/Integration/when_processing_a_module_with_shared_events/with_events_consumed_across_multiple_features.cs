// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.Integration.when_processing_a_module_with_shared_events;

/// <summary>
/// End-to-end scenario: a single SalesTracker module where the same domain events are
/// produced by a StateChange slice and then consumed by three other slices in the same
/// feature — a StateView that builds order read models, a second StateView that
/// accumulates sales statistics, and an Automation slice that triggers fulfilment.
/// The test exercises the engine's ability to fan out the same event types across multiple
/// slice types within one feature and produce a fully compilable output.
/// </summary>
public class with_events_consumed_across_multiple_features : given.a_real_engine
{
    IEnumerable<Module> _modules;

    void Establish()
    {
        // ── Shared events ─────────────────────────────────────────────────────────
        var orderPlacedEvent = new EventType(
            "SalesOrderPlaced",
            "A sales order was placed",
            [
                new Property("OrderId", "string"),
                new Property("CustomerId", "string"),
                new Property("Region", "string"),
                new Property("Amount", "decimal")
            ]);

        var orderFulfilledEvent = new EventType(
            "SalesOrderFulfilled",
            "A sales order was fulfilled",
            [
                new Property("OrderId", "string"),
                new Property("FulfilledBy", "string")
            ]);

        // ── Slice 1: StateChange — produces both events ───────────────────────────
        var placeOrderCommand = new Command(
            "PlaceSalesOrder",
            "Places a new sales order",
            [
                new Property("CustomerId", "string"),
                new Property("Region", "string"),
                new Property("Amount", "decimal")
            ],
            "OrderId");

        var fulfilCommand = new Command(
            "FulfilSalesOrder",
            "Marks a sales order as fulfilled",
            [new Property("FulfilledBy", "string")],
            "OrderId");

        var ordersSlice = new VerticalSlice(
            "SalesOrders",
            VerticalSliceType.StateChange,
            null,
            null,
            [placeOrderCommand, fulfilCommand],
            [],
            [orderPlacedEvent, orderFulfilledEvent]);

        // ── Slice 2: StateView — per-order read model ─────────────────────────────
        var orderIdMappings = new[] { new EventPropertyMapping("SalesOrderPlaced", EventPropertyMappingKind.Set, "OrderId") };
        var customerIdMappings = new[] { new EventPropertyMapping("SalesOrderPlaced", EventPropertyMappingKind.Set, "CustomerId") };
        var amountMappings = new[] { new EventPropertyMapping("SalesOrderPlaced", EventPropertyMappingKind.Set, "Amount") };
        var placedAtMappings = new[] { new EventPropertyMapping("SalesOrderPlaced", EventPropertyMappingKind.SetFromContext, "Occurred") };

        var orderDetail = new ReadModel(
            "OrderDetail",
            "Full detail for a sales order",
            [
                new ReadModelProperty("OrderId", "string", orderIdMappings),
                new ReadModelProperty("CustomerId", "string", customerIdMappings),
                new ReadModelProperty("Amount", "decimal", amountMappings),
                new ReadModelProperty("PlacedAt", "DateTimeOffset", placedAtMappings)
            ]);

        var orderDetailSlice = new VerticalSlice(
            "OrderDetails",
            VerticalSliceType.StateView,
            null,
            null,
            [],
            [orderDetail],
            [orderPlacedEvent]);

        // ── Slice 3: StateView — regional statistics, uses Add + Increment ────────
        var regionMappings = new[] { new EventPropertyMapping("SalesOrderPlaced", EventPropertyMappingKind.Set, "Region") };
        var totalOrdersMappings = new[] { new EventPropertyMapping("SalesOrderPlaced", EventPropertyMappingKind.Count) };
        var totalAmountMappings = new[] { new EventPropertyMapping("SalesOrderPlaced", EventPropertyMappingKind.Add, "Amount") };
        var fulfilledMappings = new[] { new EventPropertyMapping("SalesOrderFulfilled", EventPropertyMappingKind.Increment) };

        var regionalStats = new ReadModel(
            "RegionalSalesStats",
            "Accumulated sales statistics per region",
            [
                new ReadModelProperty("Region", "string", regionMappings),
                new ReadModelProperty("TotalOrders", "int", totalOrdersMappings),
                new ReadModelProperty("TotalAmount", "decimal", totalAmountMappings),
                new ReadModelProperty("FulfilledOrders", "int", fulfilledMappings)
            ]);

        var salesStatsSlice = new VerticalSlice(
            "SalesStats",
            VerticalSliceType.StateView,
            null,
            null,
            [],
            [regionalStats],
            [orderPlacedEvent, orderFulfilledEvent]);

        // ── Slice 4: Automation — reacts to placed orders ─────────────────────────
        var pendingOrderIdMappings = new[] { new EventPropertyMapping("SalesOrderPlaced", EventPropertyMappingKind.Set, "OrderId") };
        var pendingCustomerMappings = new[] { new EventPropertyMapping("SalesOrderPlaced", EventPropertyMappingKind.Set, "CustomerId") };
        var pendingAmountMappings = new[] { new EventPropertyMapping("SalesOrderPlaced", EventPropertyMappingKind.Set, "Amount") };

        var pendingOrder = new ReadModel(
            "PendingOrder",
            "A placed order awaiting fulfilment",
            [
                new ReadModelProperty("OrderId", "string", pendingOrderIdMappings),
                new ReadModelProperty("CustomerId", "string", pendingCustomerMappings),
                new ReadModelProperty("Amount", "decimal", pendingAmountMappings)
            ]);

        var assignFulfilmentCommand = new Command(
            "AssignFulfilment",
            "Assigns an order to a fulfilment agent",
            [new Property("FulfilledBy", "string")],
            "OrderId");

        var fulfilmentAutomation = new VerticalSlice(
            "FulfilmentAutomation",
            VerticalSliceType.Automation,
            null,
            null,
            [assignFulfilmentCommand],
            [pendingOrder],
            []);

        var salesFeature = new Feature("Sales", [], [], [ordersSlice, orderDetailSlice, salesStatsSlice, fulfilmentAutomation]);

        _modules = [new Module("SalesTracker", [], [salesFeature])];
    }

    async Task Because()
    {
        await _engine.Process(_modules, _output);
        _generatedFiles = _engine.Preview(_modules);
        AddGlobalUsingsFromGeneratedFiles();
        _buildExitCode = await RunDotnet("build");
    }

    [Fact] void should_generate_event_type_files() =>
        new[] { "SalesOrderPlaced.cs", "SalesOrderFulfilled.cs" }
            .All(n => _generatedFiles.Any(f => f.RelativePath.EndsWith(n)))
            .ShouldBeTrue();

    [Fact] void should_generate_state_change_command_files() =>
        new[] { "PlaceSalesOrder.cs", "FulfilSalesOrder.cs" }
            .All(n => _generatedFiles.Any(f => f.RelativePath.EndsWith(n)))
            .ShouldBeTrue();

    [Fact] void should_generate_order_detail_view_files() =>
        new[] { "OrderDetail.cs", "AllOrderDetails.cs" }
            .All(n => _generatedFiles.Any(f => f.RelativePath.EndsWith(n)))
            .ShouldBeTrue();

    [Fact] void should_generate_sales_stats_view_files() =>
        new[] { "RegionalSalesStats.cs", "AllRegionalSalesStatss.cs" }

            .All(n => _generatedFiles.Any(f => f.RelativePath.EndsWith(n)))
            .ShouldBeTrue();

    [Fact] void should_generate_fulfilment_automation_files() =>
        new[] { "PendingOrder.cs", "AllPendingOrders.cs", "AssignFulfilment.cs" }
            .All(n => _generatedFiles.Any(f => f.RelativePath.EndsWith(n)))
            .ShouldBeTrue();

    [Fact] void should_compile_successfully() => _buildExitCode.ShouldEqual(0);
}
