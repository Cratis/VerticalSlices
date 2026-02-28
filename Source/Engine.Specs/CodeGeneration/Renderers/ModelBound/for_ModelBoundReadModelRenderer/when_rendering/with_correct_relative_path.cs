// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundReadModelRenderer.when_rendering;

public class with_correct_relative_path : given.a_context
{
    ModelBoundReadModelRenderer _renderer;
    ReadModelDescriptor _descriptor;

    void Establish()
    {
        _renderer = new ModelBoundReadModelRenderer();
        _descriptor = new ReadModelDescriptor(
            "Employee",
            "An employee",
            [new ReadModelPropertyDescriptor("Id", "string", IsKey: true, [])],
            []);
    }

    void Because() { }

    [Fact] void should_place_projection_file_under_context_relative_path() =>
        _renderer.Render(_descriptor, _context)
            .Single(f => f.RelativePath.EndsWith("Employee.cs"))
            .RelativePath.ShouldEqual(Path.Combine(_context.RelativePath, "Employee.cs"));

    [Fact] void should_place_query_file_under_context_relative_path() =>
        _renderer.Render(_descriptor, _context)
            .Single(f => f.RelativePath.EndsWith("AllEmployees.cs"))
            .RelativePath.ShouldEqual(Path.Combine(_context.RelativePath, "AllEmployees.cs"));
}
