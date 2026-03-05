// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;

namespace Cratis.VerticalSlices.for_CodeWriter.when_formatting_record_parameters_multi_line;

public class with_single_property : Specification
{
    string _result;

    void Because() => _result = CodeWriter.FormatRecordParametersMultiLine([new Property("Name", "string")]);

    [Fact] void should_indent_the_parameter() => _result.ShouldContain("    string Name");
    [Fact] void should_not_have_trailing_comma() => _result.TrimEnd().EndsWith(',').ShouldBeFalse();
}
