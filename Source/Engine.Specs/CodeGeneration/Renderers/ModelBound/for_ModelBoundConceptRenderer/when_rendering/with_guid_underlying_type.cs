// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundConceptRenderer.when_rendering;

public class with_guid_underlying_type : given.a_context
{
    ModelBoundConceptRenderer _renderer;
    ConceptDescriptor _descriptor;
    string _conceptContent;

    void Establish()
    {
        _renderer = new ModelBoundConceptRenderer();
        _descriptor = new ConceptDescriptor("EmployeeId", "Guid", "An employee identifier", []);
    }

    void Because() => _conceptContent = _renderer.Render(_descriptor, _context)
        .Single(f => f.ArtifactPath.EndsWith("EmployeeId.cs")).Content;

    [Fact] void should_emit_concept_record() => _conceptContent.ShouldContain("public record EmployeeId(Guid Value) : ConceptAs<Guid>(Value)");
    [Fact] void should_emit_not_set_with_guid_empty() => _conceptContent.ShouldContain("NotSet = new(Value: Guid.Empty)");
}
