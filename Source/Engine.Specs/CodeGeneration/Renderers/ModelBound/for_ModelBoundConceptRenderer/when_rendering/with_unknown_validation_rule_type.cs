// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundConceptRenderer.when_rendering;

/// <summary>
/// A concept whose only validation rule has an unrecognised rule type should still
/// produce validator content in the concept file (because HasValidation is true), but the rule body
/// must be empty — no RuleFor call should be emitted for an unknown rule kind.
/// </summary>
public class with_unknown_validation_rule_type : given.a_context
{
    ModelBoundConceptRenderer _renderer;
    ConceptDescriptor _descriptor;
    string _validatorContent;

    void Establish()
    {
        _renderer = new ModelBoundConceptRenderer();

        // Use an enum value not defined in the switch expression to exercise the _ => null fallthrough
        const ConceptValidationRuleType unknownRuleType = (ConceptValidationRuleType)999;
        _descriptor = new ConceptDescriptor("CustomValue", "int", "A custom value", [new ConceptValidationRule(unknownRuleType, "42")]);
    }

    void Because() => _validatorContent = _renderer.Render(_descriptor, _context)
        .Single(f => f.ArtifactPath.EndsWith("CustomValue.cs")).Content;

    [Fact] void should_include_validator_in_concept_file() =>
        _validatorContent.ShouldNotBeNull();

    [Fact] void should_not_emit_any_rule_for_call() =>
        _validatorContent.ShouldNotContain("RuleFor(x => x.Value)");
}
