// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.for_ConceptDescriptor.when_checking_has_validation;

public class without_validation_rules : Specification
{
    ConceptDescriptor _descriptor;
    bool _result;

    void Establish() => _descriptor = new ConceptDescriptor("Name", "string", "A name", []);

    void Because() => _result = _descriptor.HasValidation;

    [Fact] void should_return_false() => _result.ShouldBeFalse();
}
