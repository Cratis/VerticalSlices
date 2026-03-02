// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;

namespace Cratis.VerticalSlices.for_SliceValidator.when_validating;

public class with_no_slices : Specification
{
    static Module[] _modules;
    Exception _exception;

    void Establish() => _modules =
    [
        new Module("TestModule", [], [new Feature("TestFeature", [], [], [])])
    ];

    void Because()
    {
        _exception = Catch.Exception(() => new SliceValidator(new EventModelAdvisor()).Validate(_modules));
    }

    [Fact] void should_not_throw() => _exception.ShouldBeNull();
}
