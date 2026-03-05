// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundEventTypeRenderer.when_rendering;

public class without_a_description : given.a_context
{
    ModelBoundEventTypeRenderer _renderer;
    EventTypeDescriptor _descriptor;
    string _content;

    void Establish()
    {
        _renderer = new ModelBoundEventTypeRenderer();
        _descriptor = new EventTypeDescriptor("OrderPlaced", string.Empty, []);
    }

    void Because() => _content = _renderer.Render(_descriptor, _context).Single().Content;

    [Fact] void should_not_include_xml_summary_comment() => _content.ShouldNotContain("/// <summary>");
}
