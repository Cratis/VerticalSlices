// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundCommandRenderer.when_rendering;

public class without_produced_events : given.a_context
{
    ModelBoundCommandRenderer _renderer;
    CommandDescriptor _descriptor;
    string _content;

    void Establish()
    {
        _renderer = new ModelBoundCommandRenderer();
        _descriptor = new CommandDescriptor(
            "PlaceOrder",
            "Places a new order",
            [new CommandPropertyDescriptor("OrderId", "string", null, null)],
            [],
            "OrderId");
    }

    void Because() => _content = _renderer.Render(_descriptor, _context).Single().Content;

    [Fact] void should_emit_command_attribute() => _content.ShouldContain("[Command]");
    [Fact] void should_emit_record_declaration() => _content.ShouldContain("public record PlaceOrder(");
    [Fact] void should_include_event_source_id_property() => _content.ShouldContain("EventSourceId OrderId");
    [Fact] void should_include_chronicle_events_using() => _content.ShouldContain("using Cratis.Chronicle.Events;");
    [Fact] void should_emit_void_handle_method() => _content.ShouldContain("public void Handle()");
    [Fact] void should_not_emit_task_handle_method() => _content.ShouldNotContain("public Task Handle(");
    [Fact] void should_declare_correct_namespace() => _content.ShouldContain("namespace MyModule.MyFeature;");
}
