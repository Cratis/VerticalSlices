// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.Controller.for_ControllerCommandRenderer.when_rendering;

/// <summary>
/// Specs for rendering a command with a controller and a produced event.
/// </summary>
public class with_a_produced_event : given.a_context
{
    ControllerCommandRenderer _renderer;
    CommandDescriptor _descriptor;
    IEnumerable<RenderedArtifact> _artifacts;

    void Establish()
    {
        _renderer = new ControllerCommandRenderer();
        _descriptor = new CommandDescriptor(
            "PlaceOrder",
            "Places an order",
            [new CommandPropertyDescriptor("ProductName", "string", null, null)],
            [new ProducedEventDescriptor(
                new EventTypeDescriptor("OrderPlaced", "An order was placed", [new Property("OrderId", "string"), new Property("ProductName", "string")]),
                [
                    new ResolvedPropertyMap("OrderId", ResolvedPropertyMapSource.FromCommand, "OrderId"),
                    new ResolvedPropertyMap("ProductName", ResolvedPropertyMapSource.FromCommand, "ProductName")
                ])],
            "OrderId");
    }

    void Because() => _artifacts = _renderer.Render(_descriptor, _context);

    [Fact] void should_produce_two_files() => _artifacts.Count().ShouldEqual(2);

    [Fact]
    void should_produce_command_record_file() =>
        _artifacts.First().Content.ShouldContain("public record PlaceOrder(");

    [Fact]
    void should_not_include_command_attribute_on_record() =>
        _artifacts.First().Content.ShouldNotContain("[Command]");

    [Fact]
    void should_produce_controller_file() =>
        _artifacts.Last().Content.ShouldContain("[ApiController]");

    [Fact]
    void should_include_http_post_attribute() =>
        _artifacts.Last().Content.ShouldContain("[HttpPost]");

    [Fact]
    void should_include_event_log_append() =>
        _artifacts.Last().Content.ShouldContain("await eventLog.Append(");

    [Fact]
    void should_map_product_name_from_command() =>
        _artifacts.Last().Content.ShouldContain("command.ProductName");
}
