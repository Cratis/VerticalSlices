// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundCommandRenderer.when_rendering;

/// <summary>
/// When a produced event has properties that overlap with command properties, the
/// Handle() method should map matching arguments by name. Properties not present
/// on the command should be filled with <c>default!</c>.
/// </summary>
public class with_produced_event_mapping_args : given.a_context
{
    ModelBoundCommandRenderer _renderer;
    CommandDescriptor _descriptor;
    string _content;

    void Establish()
    {
        _renderer = new ModelBoundCommandRenderer();
        var producedEvent = new ProducedEventDescriptor(
            new EventTypeDescriptor(
                "OrderPlaced",
                "An order was placed",
                [
                    new Property("OrderId", "string"),
                    new Property("Name", "string"),
                    new Property("Region", "string")
                ]),
            [
                new ResolvedPropertyMap("OrderId", ResolvedPropertyMapSource.FromCommand, "OrderId"),
                new ResolvedPropertyMap("Name", ResolvedPropertyMapSource.FromCommand, "Name"),
                new ResolvedPropertyMap("Region", ResolvedPropertyMapSource.ComputedDefault)
            ]);
        _descriptor = new CommandDescriptor(
            "PlaceOrder",
            "Places a new order",
            [
                new CommandPropertyDescriptor("OrderId", "string", null, null),
                new CommandPropertyDescriptor("Name", "string", null, null)
            ],
            [producedEvent],
            "OrderId");
    }

    void Because() => _content = _renderer.Render(_descriptor, _context).Single().Content;

    [Fact] void should_map_matching_args() => _content.ShouldContain("new(OrderId, Name, default!);");
    [Fact] void should_emit_handle_returning_event_type() => _content.ShouldContain("public OrderPlaced Handle()");
}
