// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundConceptRenderer.when_rendering;

public class with_greater_than_or_equal_to_rule : given.a_context
{
    ModelBoundConceptRenderer _renderer;
    ConceptDescriptor _descriptor;
    string _validatorContent;

    void Establish()
    {
        _renderer = new ModelBoundConceptRenderer();
        _descriptor = new ConceptDescriptor(
            "NonNegativeQuantity",
            "int",
            "A quantity that is zero or greater",
            [new ConceptValidationRule(ConceptValidationRuleType.GreaterThanOrEqualTo, "0")]);
    }

    void Because() => _validatorContent = _renderer.Render(_descriptor, _context)
        .Single(f => f.ArtifactPath.EndsWith("NonNegativeQuantityValidator.cs")).Content;

    [Fact] void should_emit_greater_than_or_equal_to_rule() => _validatorContent.ShouldContain(".GreaterThanOrEqualTo(0)");
}
