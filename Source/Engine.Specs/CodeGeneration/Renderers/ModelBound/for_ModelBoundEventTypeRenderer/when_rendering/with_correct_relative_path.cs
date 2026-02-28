// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundEventTypeRenderer.when_rendering;

public class with_correct_relative_path : given.a_context
{
    ModelBoundEventTypeRenderer _renderer;
    EventTypeDescriptor _descriptor;
    GeneratedFile _result;

    void Establish()
    {
        _renderer = new ModelBoundEventTypeRenderer();
        _descriptor = new EventTypeDescriptor("OrderPlaced", "An order was placed", []);
    }

    void Because() => _result = _renderer.Render(_descriptor, _context).Single();

    [Fact] void should_place_file_under_context_relative_path() =>
        _result.RelativePath.ShouldEqual(Path.Combine(_context.RelativePath, "OrderPlaced.cs"));
}
