// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundConceptRenderer.when_rendering;

public class with_less_than_rule : given.a_context
{
    ModelBoundConceptRenderer _renderer;
    ConceptDescriptor _descriptor;
    string _validatorContent;

    void Establish()
    {
        _renderer = new ModelBoundConceptRenderer();
        _descriptor = new ConceptDescriptor(
            "BoundedAge",
            "int",
            "An age that must be below 100",
            [new ConceptValidationRule(ConceptValidationRuleType.LessThan, "100")]);
    }

    void Because() => _validatorContent = _renderer.Render(_descriptor, _context)
        .Single(f => f.ArtifactPath.EndsWith("BoundedAgeValidator.cs")).Content;

    [Fact] void should_emit_less_than_rule() => _validatorContent.ShouldContain(".LessThan(100)");
}
