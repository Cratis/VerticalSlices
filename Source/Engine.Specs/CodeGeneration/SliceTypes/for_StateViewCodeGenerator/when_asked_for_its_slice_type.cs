// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.SliceTypes.for_StateViewCodeGenerator;

public class when_asked_for_its_slice_type : Specification
{
    StateViewCodeGenerator _generator;
    VerticalSliceType _result;

    void Establish() => _generator = new StateViewCodeGenerator();

    void Because() => _result = _generator.SliceType;

    [Fact] void should_return_state_view() => _result.ShouldEqual(VerticalSliceType.StateView);
}
