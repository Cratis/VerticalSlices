// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;

namespace Cratis.VerticalSlices.for_CodeWriter.when_formatting_record_parameters_multi_line;

public class with_multiple_properties : Specification
{
    string _result;

    void Because() => _result = CodeWriter.FormatRecordParametersMultiLine(
    [
        new Property("Name", "string"),
        new Property("Age", "int")
    ]);

    [Fact] void should_place_first_parameter_before_second() => _result.IndexOf("string Name", StringComparison.Ordinal).ShouldBeLessThan(_result.IndexOf("int Age", StringComparison.Ordinal));
    [Fact] void should_add_comma_after_all_but_last_parameter() => _result.ShouldContain("string Name,");
    [Fact] void should_not_have_comma_after_last_parameter() => _result.TrimEnd().EndsWith(',').ShouldBeFalse();
    [Fact] void should_indent_each_parameter() => _result.Split('\n', StringSplitOptions.RemoveEmptyEntries).All(line => line.StartsWith("    ")).ShouldBeTrue();
}
