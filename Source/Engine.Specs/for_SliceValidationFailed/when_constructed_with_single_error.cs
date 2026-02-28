// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.for_SliceValidationFailed;

public class when_constructed_with_single_error : Specification
{
    SliceValidationError _error;
    SliceValidationFailed _exception;

    void Establish() => _error = new SliceValidationError("MySlice", VerticalSliceType.StateChange, "Something is wrong");

    void Because() => _exception = new SliceValidationFailed([_error]);

    [Fact] void should_expose_errors() => _exception.Errors.ShouldContainOnly(_error);
    [Fact] void should_include_error_count_in_message() => _exception.Message.ShouldContain("1");
    [Fact] void should_include_slice_type_in_message() => _exception.Message.ShouldContain("StateChange");
    [Fact] void should_include_slice_name_in_message() => _exception.Message.ShouldContain("MySlice");
    [Fact] void should_include_violation_message_in_message() => _exception.Message.ShouldContain("Something is wrong");
}
