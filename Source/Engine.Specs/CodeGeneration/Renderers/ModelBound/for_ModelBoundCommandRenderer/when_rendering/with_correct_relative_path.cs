// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundCommandRenderer.when_rendering;

public class with_correct_relative_path : given.a_context
{
    ModelBoundCommandRenderer _renderer;
    CommandDescriptor _descriptor;
    GeneratedFile _result;

    void Establish()
    {
        _renderer = new ModelBoundCommandRenderer();
        _descriptor = new CommandDescriptor("PlaceOrder", "Places a new order", [], [], "Id");
    }

    void Because() => _result = _renderer.Render(_descriptor, _context).Single();

    [Fact] void should_place_file_under_context_relative_path() =>
        _result.RelativePath.ShouldEqual(Path.Combine(_context.RelativePath, "PlaceOrder.cs"));
}
