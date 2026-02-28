// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundCommandRenderer.when_rendering;

public class with_produced_events : given.a_context
{
    ModelBoundCommandRenderer _renderer;
    CommandDescriptor _descriptor;
    string _content;

    void Establish()
    {
        _renderer = new ModelBoundCommandRenderer();
        var producedEvent = new EventTypeDescriptor("OrderPlaced", "An order was placed", []);
        _descriptor = new CommandDescriptor(
            "PlaceOrder",
            "Places a new order",
            [new CommandPropertyDescriptor("OrderId", "string", null, null)],
            [producedEvent],
            "OrderId");
    }

    void Because() => _content = _renderer.Render(_descriptor, _context).Single().Content;

    [Fact] void should_emit_handle_returning_event_type() => _content.ShouldContain("public OrderPlaced Handle()");
    [Fact] void should_include_stub_comment_for_produced_event() => _content.ShouldContain("new OrderPlaced(");
    [Fact] void should_not_emit_void_handle_method() => _content.ShouldNotContain("public void Handle()");
    [Fact] void should_not_emit_ieventlog_parameter() => _content.ShouldNotContain("IEventLog");
}
