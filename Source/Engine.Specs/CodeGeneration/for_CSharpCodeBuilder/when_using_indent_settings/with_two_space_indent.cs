// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.for_CSharpCodeBuilder.when_using_indent_settings;

/// <summary>
/// Specs for <see cref="CSharpCodeBuilder"/> with custom indent size (2 spaces).
/// </summary>
public class with_two_space_indent : Specification
{
    CSharpCodeBuilder _builder;
    string _result;

    void Establish()
    {
        var options = new CodeGenerationOptions { IndentSize = 2 };
        var context = new CodeGenerationContext("M", FeaturePath.Empty, "S", options);
        _builder = new CSharpCodeBuilder(context);
    }

    void Because()
    {
        _builder
            .Namespace("Test.Namespace")
            .BlankLine()
            .OpenRecord("Foo", "string Bar")
            .Statement("// inner")
            .EndBlock();
        _result = _builder.ToString();
    }

    [Fact] void should_use_two_spaces_for_indentation() => _result.ShouldContain("  // inner");
    [Fact] void should_not_use_four_spaces_for_indentation() => _result.ShouldNotContain("    // inner");
}
