// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.Integration.when_processing_multiple_modules;

/// <summary>
/// End-to-end scenario: three fully independent modules — Catalog, Orders and Notifications —
/// are processed together in a single engine invocation.  Each module has its own features,
/// events and slice types with no shared types across module boundaries.  The test verifies
/// that the engine generates correct output for every module and that all of the files compile
/// together in one <c>dotnet build</c>.
/// </summary>
public class with_three_independent_modules : given.a_real_engine
{
    IEnumerable<Module> _modules;

    void Establish()
    {
        // ── Catalog module ───────────────────────────────────────────────────────
        var productListedEvent = new EventType(
            "ProductListed",
            "A product was listed in the catalog",
            [
                new Property("ProductId", "string"),
                new Property("Name", "string"),
                new Property("Price", "decimal")
            ]);

        var listProductCommand = new Command(
            "ListProduct",
            "Lists a new product in the catalog",
            [
                new Property("Name", "string"),
                new Property("Price", "decimal")
            ],
            "ProductId");

        var catalogStateChange = new VerticalSlice(
            "ListProduct",
            VerticalSliceType.StateChange,
            null,
            null,
            [listProductCommand],
            [],
            [productListedEvent]);

        var productIdMappings = new[] { new EventPropertyMapping("ProductListed", EventPropertyMappingKind.Set, "ProductId") };
        var productNameMappings = new[] { new EventPropertyMapping("ProductListed", EventPropertyMappingKind.Set, "Name") };
        var productPriceMappings = new[] { new EventPropertyMapping("ProductListed", EventPropertyMappingKind.Set, "Price") };

        var catalogEntry = new ReadModel(
            "CatalogEntry",
            "A product as it appears in the catalog",
            [
                new ReadModelProperty("ProductId", "string", productIdMappings),
                new ReadModelProperty("Name", "string", productNameMappings),
                new ReadModelProperty("Price", "decimal", productPriceMappings)
            ]);

        var catalogView = new VerticalSlice(
            "CatalogView",
            VerticalSliceType.StateView,
            null,
            null,
            [],
            [catalogEntry],
            [productListedEvent]);

        var catalogFeature = new Feature("Catalog", [], [], [catalogStateChange, catalogView]);
        var catalogModule = new Module("Catalog", [], [catalogFeature]);

        // ── Orders module ────────────────────────────────────────────────────────
        var orderId = new Concept("OrderId", "Guid", "Identifies an order", []);

        var orderPlacedEvent = new EventType(
            "OrderPlaced",
            "A customer placed an order",
            [
                new Property("OrderId", "string"),
                new Property("CustomerId", "string"),
                new Property("TotalAmount", "decimal")
            ]);

        var orderShippedEvent = new EventType(
            "OrderShipped",
            "An order was shipped",
            [
                new Property("OrderId", "string"),
                new Property("TrackingNumber", "string")
            ]);

        var placeOrderCommand = new Command(
            "PlaceOrder",
            "Places a new customer order",
            [
                new Property("CustomerId", "string"),
                new Property("TotalAmount", "decimal")
            ],
            "OrderId");

        var shipOrderCommand = new Command(
            "ShipOrder",
            "Ships an existing order",
            [new Property("TrackingNumber", "string")],
            "OrderId");

        var placeOrderSlice = new VerticalSlice(
            "PlaceOrder",
            VerticalSliceType.StateChange,
            null,
            null,
            [placeOrderCommand],
            [],
            [orderPlacedEvent]);

        var shipOrderSlice = new VerticalSlice(
            "ShipOrder",
            VerticalSliceType.StateChange,
            null,
            null,
            [shipOrderCommand],
            [],
            [orderShippedEvent]);

        var orderIdForView = new[] { new EventPropertyMapping("OrderPlaced", EventPropertyMappingKind.Set, "OrderId") };
        var customerIdMappings = new[] { new EventPropertyMapping("OrderPlaced", EventPropertyMappingKind.Set, "CustomerId") };
        var totalMappings = new[] { new EventPropertyMapping("OrderPlaced", EventPropertyMappingKind.Set, "TotalAmount") };
        var trackingNumberMappings = new[] { new EventPropertyMapping("OrderShipped", EventPropertyMappingKind.Set, "TrackingNumber") };

        var orderSummary = new ReadModel(
            "OrderSummary",
            "A summary of a placed order, enriched with tracking info once shipped",
            [
                new ReadModelProperty("OrderId", "string", orderIdForView),
                new ReadModelProperty("CustomerId", "string", customerIdMappings),
                new ReadModelProperty("TotalAmount", "decimal", totalMappings),
                new ReadModelProperty("TrackingNumber", "string", trackingNumberMappings)
            ]);

        var orderView = new VerticalSlice(
            "OrderView",
            VerticalSliceType.StateView,
            null,
            null,
            [],
            [orderSummary],
            [orderPlacedEvent, orderShippedEvent]);

        var ordersFeature = new Feature("Orders", [], [], [placeOrderSlice, shipOrderSlice, orderView]);
        var ordersModule = new Module("Orders", [orderId], [ordersFeature]);

        // ── Notifications module ─────────────────────────────────────────────────
        var externalEmailEvent = new EventType(
            "ExternalEmailDelivered",
            "An email delivery confirmation from the external email provider",
            [
                new Property("MessageId", "string"),
                new Property("Recipient", "string")
            ],
            EventKind.External);

        var emailDeliveredEvent = new EventType(
            "EmailDelivered",
            "An email was confirmed as delivered",
            [
                new Property("NotificationId", "string"),
                new Property("Recipient", "string")
            ],
            EventKind.Internal);

        var translatorSlice = new VerticalSlice(
            "IngestEmailDelivery",
            VerticalSliceType.Translator,
            null,
            null,
            [],
            [],
            [externalEmailEvent, emailDeliveredEvent]);

        var notificationIdMappings = new[] { new EventPropertyMapping("EmailDelivered", EventPropertyMappingKind.Set, "NotificationId") };
        var recipientMappings = new[] { new EventPropertyMapping("EmailDelivered", EventPropertyMappingKind.Set, "Recipient") };
        var deliveredAtMappings = new[] { new EventPropertyMapping("EmailDelivered", EventPropertyMappingKind.SetFromContext, "Occurred") };

        var deliveredNotification = new ReadModel(
            "DeliveredNotification",
            "A notification that has been delivered",
            [
                new ReadModelProperty("NotificationId", "string", notificationIdMappings),
                new ReadModelProperty("Recipient", "string", recipientMappings),
                new ReadModelProperty("DeliveredAt", "DateTimeOffset", deliveredAtMappings)
            ]);

        var sendRetryCommand = new Command(
            "RetryNotification",
            "Retries a failed notification delivery",
            [],
            "NotificationId");

        var automationSlice = new VerticalSlice(
            "RetryFailedDeliveries",
            VerticalSliceType.Automation,
            null,
            null,
            [sendRetryCommand],
            [deliveredNotification],
            []);

        var notificationsFeature = new Feature("Notifications", [], [], [translatorSlice, automationSlice]);
        var notificationsModule = new Module("Notifications", [], [notificationsFeature]);

        _modules = [catalogModule, ordersModule, notificationsModule];
    }

    async Task Because()
    {
        await _engine.Process(_modules, _output);
        _generatedFiles = _engine.Preview(_modules);
        AddGlobalUsingsFromRenderedArtifacts();
        _buildExitCode = await RunDotnet("build");
    }

    [Fact] void should_generate_catalog_module_files() =>
        new[] { "ProductListed.cs", "ListProduct.cs", "CatalogEntry.cs" }
            .All(n => _generatedFiles.Any(f => f.ArtifactPath.EndsWith(n)))
            .ShouldBeTrue();

    [Fact] void should_generate_orders_module_files() =>
        new[] { "OrderId.cs", "OrderPlaced.cs", "OrderShipped.cs", "PlaceOrder.cs", "ShipOrder.cs", "OrderSummary.cs" }
            .All(n => _generatedFiles.Any(f => f.ArtifactPath.EndsWith(n)))
            .ShouldBeTrue();

    [Fact] void should_generate_notifications_module_internal_event_file() =>
        _generatedFiles.Any(f => f.ArtifactPath.EndsWith("EmailDelivered.cs")).ShouldBeTrue();

    [Fact] void should_not_generate_external_event_file() =>
        _generatedFiles.Any(f => f.ArtifactPath.EndsWith("ExternalEmailDelivered.cs")).ShouldBeFalse();

    [Fact] void should_generate_notifications_automation_files() =>
        new[] { "DeliveredNotification.cs", "RetryNotification.cs" }
            .All(n => _generatedFiles.Any(f => f.ArtifactPath.EndsWith(n)))
            .ShouldBeTrue();

    [Fact] void should_compile_successfully() => _buildExitCode.ShouldEqual(0);
}
