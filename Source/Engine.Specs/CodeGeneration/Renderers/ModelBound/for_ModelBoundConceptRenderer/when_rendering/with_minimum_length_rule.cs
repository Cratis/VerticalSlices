// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundConceptRenderer.when_rendering;

public class with_minimum_length_rule : given.a_context
{
    ModelBoundConceptRenderer _renderer;
    ConceptDescriptor _descriptor;
    string _validatorContent;

    void Establish()
    {
        _renderer = new ModelBoundConceptRenderer();
        _descriptor = new ConceptDescriptor(
            "Username",
            "string",
            "A username",
            [new ConceptValidationRule(ConceptValidationRuleType.MinimumLength, "3")]);
    }

    void Because() => _validatorContent = _renderer.Render(_descriptor, _context)
        .Single(f => f.RelativePath.EndsWith("UsernameValidator.cs")).Content;

    [Fact] void should_emit_minimum_length_rule() => _validatorContent.ShouldContain(".MinimumLength(3)");
}
