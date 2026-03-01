// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.for_CSharpCodeBuilder.when_using_indent_settings;

/// <summary>
/// Specs for <see cref="CSharpCodeBuilder"/> with tab indentation.
/// </summary>
public class with_tabs : Specification
{
    CSharpCodeBuilder _builder;
    string _result;

    void Establish()
    {
        var options = new CodeGenerationOptions { UseTabs = true };
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

    [Fact] void should_use_tab_for_indentation() => _result.ShouldContain("\t// inner");
    [Fact] void should_not_use_spaces_for_indentation() => _result.ShouldNotContain("    // inner");
}
