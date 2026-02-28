// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;

namespace Cratis.VerticalSlices.for_CodeWriter.when_formatting_record_parameters;

public class with_multiple_properties : Specification
{
    string _result;

    void Because() => _result = CodeWriter.FormatRecordParameters(
    [
        new Property("Name", "string"),
        new Property("Age", "int")
    ]);

    [Fact] void should_separate_parameters_with_comma_and_space() => _result.ShouldEqual("string Name, int Age");
}
