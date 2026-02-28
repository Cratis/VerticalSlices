// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;

namespace Cratis.VerticalSlices.for_CodeWriter.when_formatting_usings;

public class with_unsorted_namespaces : Specification
{
    string _result;

    void Because() => _result = CodeWriter.FormatUsings(["System.Text", "Cratis.Core", "Microsoft.Extensions"]);

    [Fact] void should_place_cratis_before_microsoft() => _result.IndexOf("Cratis.Core", StringComparison.Ordinal).ShouldBeLessThan(_result.IndexOf("Microsoft.Extensions", StringComparison.Ordinal));
    [Fact] void should_place_microsoft_before_system() => _result.IndexOf("Microsoft.Extensions", StringComparison.Ordinal).ShouldBeLessThan(_result.IndexOf("System.Text", StringComparison.Ordinal));
    [Fact] void should_produce_a_using_directive_for_each_namespace() => _result.Split('\n', StringSplitOptions.RemoveEmptyEntries).Length.ShouldEqual(3);
}
