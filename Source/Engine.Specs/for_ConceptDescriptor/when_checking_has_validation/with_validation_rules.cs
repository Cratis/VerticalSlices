// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.for_ConceptDescriptor.when_checking_has_validation;

public class with_validation_rules : Specification
{
    ConceptDescriptor _descriptor;
    bool _result;

    void Establish() => _descriptor = new ConceptDescriptor(
        "Email",
        "string",
        "An email address",
        [new ConceptValidationRule(ConceptValidationRuleType.EmailAddress)]);

    void Because() => _result = _descriptor.HasValidation;

    [Fact] void should_return_true() => _result.ShouldBeTrue();
}
