// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundConceptRenderer.when_rendering;

public class with_maximum_length_rule : given.a_context
{
    ModelBoundConceptRenderer _renderer;
    ConceptDescriptor _descriptor;
    string _validatorContent;

    void Establish()
    {
        _renderer = new ModelBoundConceptRenderer();
        _descriptor = new ConceptDescriptor(
            "ShortDescription",
            "string",
            "A short description",
            [new ConceptValidationRule(ConceptValidationRuleType.MaximumLength, "100")]);
    }

    void Because() => _validatorContent = _renderer.Render(_descriptor, _context)
        .Single(f => f.ArtifactPath.EndsWith("ShortDescription.cs")).Content;

    [Fact] void should_emit_maximum_length_rule() => _validatorContent.ShouldContain(".MaximumLength(100)");
}
