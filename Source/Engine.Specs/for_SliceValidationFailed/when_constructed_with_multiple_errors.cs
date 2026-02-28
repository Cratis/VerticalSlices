// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.for_SliceValidationFailed;

public class when_constructed_with_multiple_errors : Specification
{
    SliceValidationError _firstError;
    SliceValidationError _secondError;
    SliceValidationFailed _exception;

    void Establish()
    {
        _firstError = new SliceValidationError("SliceOne", VerticalSliceType.StateChange, "First violation");
        _secondError = new SliceValidationError("SliceTwo", VerticalSliceType.StateView, "Second violation");
    }

    void Because() => _exception = new SliceValidationFailed([_firstError, _secondError]);

    [Fact] void should_expose_all_errors() => _exception.Errors.Count.ShouldEqual(2);
    [Fact] void should_include_error_count_in_message() => _exception.Message.ShouldContain("2");
    [Fact] void should_include_first_slice_name_in_message() => _exception.Message.ShouldContain("SliceOne");
    [Fact] void should_include_second_slice_name_in_message() => _exception.Message.ShouldContain("SliceTwo");
    [Fact] void should_include_first_violation_message() => _exception.Message.ShouldContain("First violation");
    [Fact] void should_include_second_violation_message() => _exception.Message.ShouldContain("Second violation");
}
