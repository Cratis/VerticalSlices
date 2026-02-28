// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.for_ConceptDescriptor.when_creating_from_concept;

public class with_a_concept : Specification
{
    Concept _concept;
    ConceptDescriptor _result;

    void Establish() => _concept = new Concept(
        "Email",
        "string",
        "An email address",
        [new ConceptValidationRule(ConceptValidationRuleType.EmailAddress)]);

    void Because() => _result = ConceptDescriptor.FromConcept(_concept);

    [Fact] void should_map_name() => _result.Name.ShouldEqual("Email");
    [Fact] void should_map_underlying_type() => _result.UnderlyingType.ShouldEqual("string");
    [Fact] void should_map_description() => _result.Description.ShouldEqual("An email address");
    [Fact] void should_map_validation_rules() => _result.ValidationRules.ShouldContainOnly(_concept.ValidationRules.First());
}
