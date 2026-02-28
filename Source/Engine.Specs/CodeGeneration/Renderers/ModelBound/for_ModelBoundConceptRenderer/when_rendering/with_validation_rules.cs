// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundConceptRenderer.when_rendering;

public class with_validation_rules : given.a_context
{
    ModelBoundConceptRenderer _renderer;
    ConceptDescriptor _descriptor;
    IEnumerable<RenderedArtifact> _result;

    void Establish()
    {
        _renderer = new ModelBoundConceptRenderer();
        _descriptor = new ConceptDescriptor(
            "EmailAddress",
            "string",
            "An email address",
            [
                new ConceptValidationRule(ConceptValidationRuleType.NotEmpty),
                new ConceptValidationRule(ConceptValidationRuleType.EmailAddress)
            ]);
    }

    void Because() => _result = _renderer.Render(_descriptor, CodeGenerationContext.FromNamespace("MyModule"));

    [Fact] void should_return_two_files() => _result.Count().ShouldEqual(2);
    [Fact] void should_generate_concept_file() => _result.Any(f => f.ArtifactPath.EndsWith("EmailAddress.cs")).ShouldBeTrue();
    [Fact] void should_generate_validator_file() => _result.Any(f => f.ArtifactPath.EndsWith("EmailAddressValidator.cs")).ShouldBeTrue();
    [Fact] void should_emit_validator_class() => _result.Single(f => f.ArtifactPath.EndsWith("EmailAddressValidator.cs")).Content.ShouldContain("public class EmailAddressValidator : ConceptValidator<EmailAddress>");
    [Fact] void should_include_not_empty_rule() => _result.Single(f => f.ArtifactPath.EndsWith("EmailAddressValidator.cs")).Content.ShouldContain(".NotEmpty()");
    [Fact] void should_include_email_address_rule() => _result.Single(f => f.ArtifactPath.EndsWith("EmailAddressValidator.cs")).Content.ShouldContain(".EmailAddress()");
}
