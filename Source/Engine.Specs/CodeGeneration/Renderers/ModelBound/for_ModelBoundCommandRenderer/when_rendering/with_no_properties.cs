// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundCommandRenderer.when_rendering;

/// <summary>
/// A command with no properties should produce a record with an empty parameter
/// list, e.g. <c>public record DoThing()</c>.
/// </summary>
public class with_no_properties : given.a_context
{
    ModelBoundCommandRenderer _renderer;
    CommandDescriptor _descriptor;
    string _content;

    void Establish()
    {
        _renderer = new ModelBoundCommandRenderer();
        _descriptor = new CommandDescriptor("PingService", "A no-arg ping command", [], [], "Id");
    }

    void Because() => _content = _renderer.Render(_descriptor, _context).Single().Content;

    [Fact] void should_emit_record_with_empty_parameter_list() =>
        _content.ShouldContain("public record PingService(EventSourceId Id)");
}
