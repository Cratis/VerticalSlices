// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundConceptRenderer.when_rendering;

public class with_email_address_rule : given.a_context
{
    ModelBoundConceptRenderer _renderer;
    ConceptDescriptor _descriptor;
    string _validatorContent;

    void Establish()
    {
        _renderer = new ModelBoundConceptRenderer();
        _descriptor = new ConceptDescriptor(
            "EmailAddress",
            "string",
            "A valid email address",
            [new ConceptValidationRule(ConceptValidationRuleType.EmailAddress, null)]);
    }

    void Because() => _validatorContent = _renderer.Render(_descriptor, _context)
        .Single(f => f.RelativePath.EndsWith("EmailAddressValidator.cs")).Content;

    [Fact] void should_emit_email_address_rule() =>
        _validatorContent.ShouldContain(".EmailAddress()");
}
