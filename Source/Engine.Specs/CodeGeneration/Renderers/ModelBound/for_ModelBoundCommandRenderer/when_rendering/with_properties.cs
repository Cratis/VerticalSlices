// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundCommandRenderer.when_rendering;

public class with_properties : given.a_context
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
            [
                new CommandPropertyDescriptor("EmployeeId", "Guid", null, null),
                new CommandPropertyDescriptor("Name", "string", null, null)
            ],
            [],
            "Id");
    }

    void Because() => _content = _renderer.Render(_descriptor, _context).Single().Content;

    [Fact] void should_include_first_property_type() => _content.ShouldContain("Guid");
    [Fact] void should_include_first_property_name() => _content.ShouldContain("EmployeeId");
    [Fact] void should_include_second_property_type() => _content.ShouldContain("string");
    [Fact] void should_include_second_property_name() => _content.ShouldContain("Name");
    [Fact] void should_emit_record_declaration() => _content.ShouldContain("public record RegisterEmployee(");
}
