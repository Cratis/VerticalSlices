// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundCommandRenderer.when_rendering;

public class with_multiple_produced_events : given.a_context
{
    ModelBoundCommandRenderer _renderer;
    CommandDescriptor _descriptor;
    string _content;

    void Establish()
    {
        _renderer = new ModelBoundCommandRenderer();
        _descriptor = new CommandDescriptor(
            "RegisterEmployee",
            "Registers a new employee",
            [new CommandPropertyDescriptor("EmployeeId", "string", null, null)],
            [
                new EventTypeDescriptor("EmployeeRegistered", "Employee was registered", []),
                new EventTypeDescriptor("EmployeeWelcomed", "Employee was welcomed", [])
            ],
            "Id");
    }

    void Because() => _content = _renderer.Render(_descriptor, _context).Single().Content;

    [Fact] void should_include_constructor_call_for_first_event() => _content.ShouldContain("new EmployeeRegistered(");
    [Fact] void should_include_constructor_call_for_second_event() => _content.ShouldContain("new EmployeeWelcomed(");
    [Fact] void should_emit_ienumerable_object_handle_method() => _content.ShouldContain("public IEnumerable<object> Handle()");
    [Fact] void should_not_emit_ieventlog_parameter() => _content.ShouldNotContain("IEventLog");
}
