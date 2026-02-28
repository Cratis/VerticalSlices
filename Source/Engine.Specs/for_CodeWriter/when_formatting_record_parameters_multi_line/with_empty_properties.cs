// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;

namespace Cratis.VerticalSlices.for_CodeWriter.when_formatting_record_parameters_multi_line;

public class with_empty_properties : Specification
{
    string _result;

    void Because() => _result = CodeWriter.FormatRecordParametersMultiLine([]);

    [Fact] void should_return_empty_string() => _result.ShouldEqual(string.Empty);
}
