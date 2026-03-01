// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.Controller.for_ControllerReadModelRenderer.when_rendering;

/// <summary>
/// Specs for rendering a read model with a controller endpoint.
/// </summary>
public class with_a_simple_read_model : given.a_context
{
    ControllerReadModelRenderer _renderer;
    ReadModelDescriptor _descriptor;
    IEnumerable<RenderedArtifact> _artifacts;

    void Establish()
    {
        _renderer = new ControllerReadModelRenderer();
        _descriptor = new ReadModelDescriptor(
            "OrderView",
            "Shows order details",
            [
                new ReadModelPropertyDescriptor("OrderId", "string", true, []),
                new ReadModelPropertyDescriptor("ProductName", "string", false, [])
            ],
            []);
    }

    void Because() => _artifacts = _renderer.Render(_descriptor, _context);

    [Fact] void should_produce_two_files() => _artifacts.Count().ShouldEqual(2);

    [Fact]
    void should_produce_projection_with_read_model_attribute() =>
        _artifacts.First().Content.ShouldContain("[ReadModel]");

    [Fact]
    void should_produce_controller_with_api_controller_attribute() =>
        _artifacts.Last().Content.ShouldContain("[ApiController]");

    [Fact]
    void should_include_http_get_attribute() =>
        _artifacts.Last().Content.ShouldContain("[HttpGet]");

    [Fact]
    void should_include_get_by_id_endpoint() =>
        _artifacts.Last().Content.ShouldContain("[HttpGet(\"{id}\")]");

    [Fact]
    void should_use_mongo_collection() =>
        _artifacts.Last().Content.ShouldContain("IMongoCollection<OrderView>");
}
