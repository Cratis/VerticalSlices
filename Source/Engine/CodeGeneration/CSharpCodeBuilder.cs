// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;

namespace Cratis.VerticalSlices.CodeGeneration;

/// <summary>
/// Fluent builder for constructing well-formatted C# source files.
/// Tracks using directives separately and emits them sorted at the top of the output.
/// Manages indentation automatically for brace-delimited blocks.
/// </summary>
/// <param name="context">
/// Optional code generation context. When provided and
/// <see cref="CodeGenerationOptions.UseGlobalUsings"/> is <see langword="true"/>, using
/// directives are suppressed from the output.
/// </param>
public class CSharpCodeBuilder(CodeGenerationContext? context = null)
{
    readonly StringBuilder _body = new();
    readonly HashSet<string> _usings = [];
    readonly bool _useGlobalUsings = context?.Options?.UseGlobalUsings == true;
    int _indentLevel;

    string CurrentIndent => new(' ', _indentLevel * 4);

    /// <summary>
    /// Adds one or more namespace using directives to the file.
    /// Duplicates are silently ignored, and directives are sorted alphabetically in the output.
    /// </summary>
    /// <param name="namespaces">The namespaces to add.</param>
    /// <returns>This builder instance for fluent chaining.</returns>
    public CSharpCodeBuilder Using(params string[] namespaces)
    {
        foreach (var ns in namespaces)
        {
            _usings.Add(ns);
        }

        return this;
    }

    /// <summary>
    /// Appends a file-scoped namespace declaration.
    /// </summary>
    /// <param name="ns">The namespace name.</param>
    /// <returns>This builder instance for fluent chaining.</returns>
    public CSharpCodeBuilder Namespace(string ns)
    {
        _body.AppendLine($"namespace {ns};");
        return this;
    }

    /// <summary>
    /// Appends a blank line.
    /// </summary>
    /// <returns>This builder instance for fluent chaining.</returns>
    public CSharpCodeBuilder BlankLine()
    {
        _body.AppendLine();
        return this;
    }

    /// <summary>
    /// Appends an XML <c>&lt;summary&gt;</c> doc comment block.
    /// </summary>
    /// <param name="text">The summary text.</param>
    /// <returns>This builder instance for fluent chaining.</returns>
    public CSharpCodeBuilder Summary(string text)
    {
        _body
            .AppendLine($"{CurrentIndent}/// <summary>")
            .AppendLine($"{CurrentIndent}/// {text}")
            .AppendLine($"{CurrentIndent}/// </summary>");
        return this;
    }

    /// <summary>
    /// Appends an XML <c>&lt;param&gt;</c> doc comment line.
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="description">The parameter description.</param>
    /// <returns>This builder instance for fluent chaining.</returns>
    public CSharpCodeBuilder XmlParam(string name, string description)
    {
        _body.AppendLine($"{CurrentIndent}/// <param name=\"{name}\">{description}</param>");
        return this;
    }

    /// <summary>
    /// Appends an XML <c>&lt;returns&gt;</c> doc comment line.
    /// </summary>
    /// <param name="description">The return value description.</param>
    /// <returns>This builder instance for fluent chaining.</returns>
    public CSharpCodeBuilder XmlReturns(string description)
    {
        _body.AppendLine($"{CurrentIndent}/// <returns>{description}</returns>");
        return this;
    }

    /// <summary>
    /// Appends an attribute applied to the next declaration, wrapped in square brackets.
    /// The caller provides the full attribute content without brackets
    /// (e.g. <c>"EventType"</c>, <c>"Command"</c>, <c>"Key"</c>,
    /// <c>"SetFrom&lt;MyEvent&gt;(nameof(MyEvent.Prop))"</c>).
    /// </summary>
    /// <param name="attribute">The attribute text, without enclosing brackets.</param>
    /// <returns>This builder instance for fluent chaining.</returns>
    public CSharpCodeBuilder Attribute(string attribute)
    {
        _body.AppendLine($"{CurrentIndent}[{attribute}]");
        return this;
    }

    /// <summary>
    /// Appends a single-line record declaration terminated with a semicolon.
    /// Use this for records whose entire primary constructor fits on one line and that have no body.
    /// For records that need a body, use <see cref="OpenRecord"/>.
    /// </summary>
    /// <param name="name">The record name.</param>
    /// <param name="parameters">Optional primary constructor parameters, e.g. <c>"string Name"</c>.</param>
    /// <param name="baseType">Optional base type or interface, e.g. <c>"ConceptAs&lt;string&gt;(Value)"</c>.</param>
    /// <returns>This builder instance for fluent chaining.</returns>
    public CSharpCodeBuilder Record(string name, string? parameters = null, string? baseType = null)
    {
        var declaration = BuildTypeDeclaration("record", name, isPartial: false, parameters, baseType);
        _body.AppendLine($"{CurrentIndent}{declaration};");
        return this;
    }

    /// <summary>
    /// Appends a record declaration and opens its body block.
    /// Use this for records that contain members (methods, properties, operators, etc.).
    /// </summary>
    /// <param name="name">The record name.</param>
    /// <param name="parameters">Optional primary constructor parameters, e.g. <c>"string Name"</c>.</param>
    /// <param name="baseType">Optional base type or interface, e.g. <c>"ConceptAs&lt;string&gt;(Value)"</c>.</param>
    /// <param name="isPartial">Whether to emit the <c>partial</c> modifier.</param>
    /// <returns>This builder instance for fluent chaining.</returns>
    public CSharpCodeBuilder OpenRecord(string name, string? parameters = null, string? baseType = null, bool isPartial = false)
    {
        var declaration = BuildTypeDeclaration("record", name, isPartial, parameters, baseType);
        _body.AppendLine($"{CurrentIndent}{declaration}")
            .AppendLine($"{CurrentIndent}{{");
        _indentLevel++;
        return this;
    }

    /// <summary>
    /// Appends a class declaration and opens its body block.
    /// </summary>
    /// <param name="name">The class name.</param>
    /// <param name="baseType">Optional base type or interface, e.g. <c>"ConceptValidator&lt;Foo&gt;"</c>.</param>
    /// <param name="isPartial">Whether to emit the <c>partial</c> modifier.</param>
    /// <returns>This builder instance for fluent chaining.</returns>
    public CSharpCodeBuilder OpenClass(string name, string? baseType = null, bool isPartial = false)
    {
        var declaration = BuildTypeDeclaration("class", name, isPartial, null, baseType);
        _body.AppendLine($"{CurrentIndent}{declaration}")
            .AppendLine($"{CurrentIndent}{{");
        _indentLevel++;
        return this;
    }

    /// <summary>
    /// Closes a record, class, or method body block that was opened by
    /// <see cref="OpenRecord"/>, <see cref="OpenClass"/>, or <see cref="OpenMethod"/>.
    /// </summary>
    /// <returns>This builder instance for fluent chaining.</returns>
    public CSharpCodeBuilder EndBlock()
    {
        _indentLevel--;
        _body.AppendLine($"{CurrentIndent}}}");
        return this;
    }

    /// <summary>
    /// Appends a method signature and opens its body block.
    /// </summary>
    /// <param name="returnType">The return type, e.g. <c>"void"</c> or <c>"IEnumerable&lt;object&gt;"</c>.</param>
    /// <param name="name">The method name.</param>
    /// <param name="parameters">Optional parameter list, e.g. <c>"string value"</c>.</param>
    /// <returns>This builder instance for fluent chaining.</returns>
    public CSharpCodeBuilder OpenMethod(string returnType, string name, string parameters = "")
    {
        _body.AppendLine($"{CurrentIndent}public {returnType} {name}({parameters})")
            .AppendLine($"{CurrentIndent}{{");
        _indentLevel++;
        return this;
    }

    /// <summary>
    /// Appends a constructor signature and opens its body block.
    /// </summary>
    /// <param name="name">The constructor name (same as the class name).</param>
    /// <param name="parameters">Optional parameter list, e.g. <c>"string value"</c>.</param>
    /// <returns>This builder instance for fluent chaining.</returns>
    public CSharpCodeBuilder OpenConstructor(string name, string parameters = "")
    {
        _body.AppendLine($"{CurrentIndent}public {name}({parameters})")
            .AppendLine($"{CurrentIndent}{{");
        _indentLevel++;
        return this;
    }

    /// <summary>
    /// Appends an expression-bodied member, placing the <c>=&gt;</c> on the signature line
    /// and the expression on the next line with one extra level of indentation.
    /// The <paramref name="expression"/> should include the trailing semicolon.
    /// </summary>
    /// <param name="returnType">The return type, e.g. <c>"Foo"</c> or <c>"ISubject&lt;Foo&gt;"</c>.</param>
    /// <param name="name">The method name.</param>
    /// <param name="expression">The expression body, e.g. <c>"new Foo(a);"</c>.</param>
    /// <param name="parameters">Optional parameter list, e.g. <c>"string value"</c>.</param>
    /// <param name="isStatic">Whether to emit the <see langword="static"/> modifier.</param>
    /// <returns>This builder instance for fluent chaining.</returns>
    public CSharpCodeBuilder ExpressionMember(string returnType, string name, string expression, string parameters = "", bool isStatic = false)
    {
        var staticPart = isStatic ? "static " : string.Empty;
        _body
            .AppendLine($"{CurrentIndent}public {staticPart}{returnType} {name}({parameters}) =>")
            .AppendLine($"{CurrentIndent}    {expression}");
        return this;
    }

    /// <summary>
    /// Appends an arbitrary code statement at the current indentation level.
    /// Use for any single line that does not have its own dedicated builder method —
    /// field declarations, inline operator expressions, comments, throw statements, etc.
    /// </summary>
    /// <param name="code">The statement text, without a trailing newline.</param>
    /// <returns>This builder instance for fluent chaining.</returns>
    public CSharpCodeBuilder Statement(string code)
    {
        _body.AppendLine($"{CurrentIndent}{code}");
        return this;
    }

    /// <summary>
    /// Begins a multi-line primary constructor parameter list for a record.
    /// Call <see cref="ConstructorParameter"/> for each parameter and
    /// pass <see langword="true"/> for <c>isLast</c> on the final one to close the list.
    /// </summary>
    /// <param name="name">The record name.</param>
    /// <param name="isPartial">Whether to emit the <c>partial</c> modifier.</param>
    /// <returns>This builder instance for fluent chaining.</returns>
    public CSharpCodeBuilder BeginPrimaryConstructorParameters(string name, bool isPartial = false)
    {
        var partial = isPartial ? "partial " : string.Empty;
        _body.AppendLine($"{CurrentIndent}public {partial}record {name}(");
        _indentLevel++;
        return this;
    }

    /// <summary>
    /// Appends a single parameter inside a multi-line primary constructor parameter list.
    /// When <paramref name="isLast"/> is <see langword="true"/>, the parameter is followed by
    /// <c>);</c> and the indentation is restored to the level before
    /// <see cref="BeginPrimaryConstructorParameters"/> was called.
    /// When <paramref name="openBody"/> is additionally <see langword="true"/>, the closing
    /// delimiter is <c>)</c> (no semicolon) and a body block is opened with <c>{</c>.
    /// </summary>
    /// <param name="declaration">The parameter declaration, e.g. <c>"string Name"</c>.</param>
    /// <param name="isLast">Whether this is the last parameter in the list.</param>
    /// <param name="openBody">
    /// When <see langword="true"/> and <paramref name="isLast"/> is also <see langword="true"/>,
    /// opens a body block after the closing parenthesis instead of emitting a semicolon.
    /// Use this for partial records that contain method members.
    /// </param>
    /// <returns>This builder instance for fluent chaining.</returns>
    public CSharpCodeBuilder ConstructorParameter(string declaration, bool isLast = false, bool openBody = false)
    {
        if (isLast)
        {
            if (openBody)
            {
                _body.AppendLine($"{CurrentIndent}{declaration})");
                _indentLevel--;
                _body.AppendLine($"{CurrentIndent}{{");
                _indentLevel++;
            }
            else
            {
                _body.AppendLine($"{CurrentIndent}{declaration});");
                _indentLevel--;
            }
        }
        else
        {
            _body.AppendLine($"{CurrentIndent}{declaration},");
        }

        return this;
    }

    /// <summary>
    /// Builds the final source text.
    /// Using directives are sorted alphabetically and emitted at the top (unless
    /// <see cref="CodeGenerationOptions.UseGlobalUsings"/> is enabled on the context), followed by
    /// a blank separator line and then the body content written via the other builder methods.
    /// </summary>
    /// <returns>The complete C# source file content as a string.</returns>
    public override string ToString()
    {
        var result = new StringBuilder();

        if (!_useGlobalUsings && _usings.Count > 0)
        {
            foreach (var ns in _usings.Order(StringComparer.Ordinal))
            {
                result.AppendLine($"using {ns};");
            }

            result.AppendLine();
        }

        result.Append(_body);
        return result.ToString();
    }

    /// <summary>
    /// Builds the declaration string for a type (record or class).
    /// </summary>
    /// <param name="keyword">The type keyword, e.g. <c>"record"</c> or <c>"class"</c>.</param>
    /// <param name="name">The type name.</param>
    /// <param name="isPartial">Whether to include the <c>partial</c> modifier.</param>
    /// <param name="parameters">Optional primary constructor parameters.</param>
    /// <param name="baseType">Optional base type.</param>
    /// <returns>The complete declaration string without a trailing newline or semicolon.</returns>
    static string BuildTypeDeclaration(string keyword, string name, bool isPartial, string? parameters, string? baseType)
    {
        var partial = isPartial ? "partial " : string.Empty;
        var paramsPart = string.IsNullOrEmpty(parameters) ? string.Empty : $"({parameters})";
        var basePart = string.IsNullOrEmpty(baseType) ? string.Empty : $" : {baseType}";
        return $"public {partial}{keyword} {name}{paramsPart}{basePart}";
    }
}
