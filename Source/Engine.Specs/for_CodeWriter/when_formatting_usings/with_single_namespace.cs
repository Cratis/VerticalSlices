// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;

namespace Cratis.VerticalSlices.for_CodeWriter.when_formatting_usings;

public class with_single_namespace : Specification
{
    string _result;

    void Because() => _result = CodeWriter.FormatUsings(["System.Linq"]);

    [Fact] void should_produce_using_directive() => _result.ShouldContain("using System.Linq;");
}
