// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundConceptRenderer.when_rendering;

public class with_validation_rule_with_custom_message : given.a_context
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
            "An email address",
            [new ConceptValidationRule(ConceptValidationRuleType.EmailAddress, Message: "Must be a valid email address")]);
    }

    void Because() => _validatorContent = _renderer.Render(_descriptor, _context)
        .Single(f => f.ArtifactPath.EndsWith("EmailAddress.cs")).Content;

    [Fact] void should_emit_email_address_rule() => _validatorContent.ShouldContain(".EmailAddress()");
    [Fact] void should_append_with_message() => _validatorContent.ShouldContain(".WithMessage(\"Must be a valid email address\")");
}
