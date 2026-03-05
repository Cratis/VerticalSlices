// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.for_ConceptDescriptor.when_creating_from_concept;

public class without_validation_rules : Specification
{
    Concept _concept;
    ConceptDescriptor _result;

    void Establish() => _concept = new Concept("EmployeeId", "Guid", "An employee identifier", []);

    void Because() => _result = ConceptDescriptor.FromConcept(_concept);

    [Fact] void should_map_name() => _result.Name.ShouldEqual("EmployeeId");
    [Fact] void should_map_underlying_type() => _result.UnderlyingType.ShouldEqual("Guid");
    [Fact] void should_map_description() => _result.Description.ShouldEqual("An employee identifier");
    [Fact] void should_have_no_validation_rules() => _result.ValidationRules.ShouldBeEmpty();
    [Fact] void should_report_has_validation_as_false() => _result.HasValidation.ShouldBeFalse();
}
