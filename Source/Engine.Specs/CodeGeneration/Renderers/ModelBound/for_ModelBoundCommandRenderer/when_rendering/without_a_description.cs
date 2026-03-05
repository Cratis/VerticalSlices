// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundCommandRenderer.when_rendering;

/// <summary>
/// When a command descriptor has no description set, the renderer should not emit
/// any XML doc comment block before the record declaration.
/// </summary>
public class without_a_description : given.a_context
{
    ModelBoundCommandRenderer _renderer;
    CommandDescriptor _descriptor;
    string _content;

    void Establish()
    {
        _renderer = new ModelBoundCommandRenderer();
        _descriptor = new CommandDescriptor("DeleteItem", null!, [], [], "Id");
    }

    void Because() => _content = _renderer.Render(_descriptor, _context).Single().Content;

    [Fact] void should_not_emit_class_level_xml_summary() =>

        // The Handle method always has a summary; we verify no summary appears *before* [Command]
        _content.IndexOf("/// <summary>", StringComparison.Ordinal)
            .ShouldBeGreaterThan(_content.IndexOf("[Command]", StringComparison.Ordinal));
}
