// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;

namespace Cratis.VerticalSlices.for_CodeWriter.when_formatting_record_parameters;

public class with_single_property : Specification
{
    string _result;

    void Because() => _result = CodeWriter.FormatRecordParameters([new Property("Name", "string")]);

    [Fact] void should_return_type_and_name() => _result.ShouldEqual("string Name");
}
