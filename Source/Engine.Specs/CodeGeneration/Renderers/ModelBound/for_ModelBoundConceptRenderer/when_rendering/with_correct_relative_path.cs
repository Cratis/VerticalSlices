// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundConceptRenderer.when_rendering;

public class with_correct_relative_path : given.a_context
{
    ModelBoundConceptRenderer _renderer;
    ConceptDescriptor _descriptor;

    void Establish()
    {
        _renderer = new ModelBoundConceptRenderer();
        _descriptor = new ConceptDescriptor("EmployeeId", "Guid", "An employee identifier", []);
    }

    void Because() { }

    [Fact] void should_place_concept_file_under_context_relative_path() =>
        _renderer.Render(_descriptor, _context)
            .Single()
            .RelativePath.ShouldEqual(Path.Combine(_context.RelativePath, "EmployeeId.cs"));
}
