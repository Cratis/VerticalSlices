// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;
using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound;

/// <summary>
/// Renders a concept descriptor as a ConceptAs record and optionally a ConceptValidator class.
/// </summary>
public class ModelBoundConceptRenderer : IArtifactRenderer<ConceptDescriptor>
{
    /// <inheritdoc/>
    public IEnumerable<GeneratedFile> Render(ConceptDescriptor descriptor, CodeGenerationContext context)
    {
        var files = new List<GeneratedFile>
        {
            RenderConcept(descriptor, context)
        };

        if (descriptor.HasValidation)
        {
            files.Add(RenderValidator(descriptor, context));
        }

        return files;
    }

    static GeneratedFile RenderConcept(ConceptDescriptor descriptor, CodeGenerationContext context)
    {
        var builder = new StringBuilder()
            .AppendLine(CodeWriter.FormatUsings(["Cratis.Concepts"]))
            .AppendLine()
            .AppendLine($"namespace {context.Namespace};");

        builder.AppendLine();
        if (!string.IsNullOrWhiteSpace(descriptor.Description))
        {
            builder
                .AppendLine("/// <summary>")
                .AppendLine($"/// {descriptor.Description}")
                .AppendLine("/// </summary>");
        }

        builder
            .AppendLine($"public record {descriptor.Name}({descriptor.UnderlyingType} Value) : ConceptAs<{descriptor.UnderlyingType}>(Value)")
            .AppendLine("{")
            .AppendLine($"    public static implicit operator {descriptor.Name}({descriptor.UnderlyingType} value) => new(value);");

        AppendStaticDefaults(builder, descriptor);

        builder.AppendLine("}");

        var relativePath = Path.Combine(context.RelativePath, $"{descriptor.Name}.cs");

        return new GeneratedFile(relativePath, builder.ToString());
    }

    static GeneratedFile RenderValidator(ConceptDescriptor descriptor, CodeGenerationContext context)
    {
        var builder = new StringBuilder()
            .AppendLine(CodeWriter.FormatUsings(["Cratis.Arc.Validation", "FluentValidation"]))
            .AppendLine()
            .AppendLine($"namespace {context.Namespace};");

        builder
            .AppendLine()
            .AppendLine("/// <summary>")
            .AppendLine($"/// Validates the <see cref=\"{descriptor.Name}\"/> concept.")
            .AppendLine("/// </summary>")
            .AppendLine($"public class {descriptor.Name}Validator : ConceptValidator<{descriptor.Name}>")
            .AppendLine("{")
            .AppendLine("    /// <summary>")
            .AppendLine($"    /// Initializes a new instance of the <see cref=\"{descriptor.Name}Validator\"/> class.")
            .AppendLine("    /// </summary>")
            .AppendLine($"    public {descriptor.Name}Validator()")
            .AppendLine("    {");

        foreach (var rule in descriptor.ValidationRules)
        {
            var ruleExpression = FormatValidationRule(rule);
            if (ruleExpression is not null)
            {
                builder.AppendLine($"        RuleFor(x => x.Value){ruleExpression};");
            }
        }

        builder
            .AppendLine("    }")
            .AppendLine("}");

        var relativePath = Path.Combine(context.RelativePath, $"{descriptor.Name}Validator.cs");

        return new GeneratedFile(relativePath, builder.ToString());
    }

    static void AppendStaticDefaults(StringBuilder builder, ConceptDescriptor descriptor)
    {
        var defaultExpression = descriptor.UnderlyingType switch
        {
            "string" => "new(string.Empty)",
            "Guid" => "new(Guid.Empty)",
            "int" or "long" or "short" or "byte"
                or "uint" or "ulong" or "ushort" or "sbyte"
                or "float" or "double" or "decimal" => "new(0)",
            "bool" => null,
            "DateTime" => "new(DateTime.MinValue)",
            "DateOnly" => "new(DateOnly.MinValue)",
            "TimeOnly" => "new(TimeOnly.MinValue)",
            "DateTimeOffset" => "new(DateTimeOffset.MinValue)",
            _ => null
        };

        if (defaultExpression is not null)
        {
            builder
                .AppendLine()
                .AppendLine($"    public static readonly {descriptor.Name} NotSet = {defaultExpression};");
        }
    }

    static string? FormatValidationRule(ConceptValidationRule rule) =>
        rule.Type switch
        {
            ConceptValidationRuleType.NotEmpty =>
                WithMessage(".NotEmpty()", rule.Message),
            ConceptValidationRuleType.MinimumLength when rule.Value is not null =>
                WithMessage($".MinimumLength({rule.Value})", rule.Message),
            ConceptValidationRuleType.MaximumLength when rule.Value is not null =>
                WithMessage($".MaximumLength({rule.Value})", rule.Message),
            ConceptValidationRuleType.EmailAddress =>
                WithMessage(".EmailAddress()", rule.Message),
            ConceptValidationRuleType.Pattern when rule.Value is not null =>
                WithMessage($".Matches(\"{EscapeString(rule.Value)}\")", rule.Message),
            ConceptValidationRuleType.GreaterThan when rule.Value is not null =>
                WithMessage($".GreaterThan({rule.Value})", rule.Message),
            ConceptValidationRuleType.LessThan when rule.Value is not null =>
                WithMessage($".LessThan({rule.Value})", rule.Message),
            ConceptValidationRuleType.GreaterThanOrEqualTo when rule.Value is not null =>
                WithMessage($".GreaterThanOrEqualTo({rule.Value})", rule.Message),
            ConceptValidationRuleType.LessThanOrEqualTo when rule.Value is not null =>
                WithMessage($".LessThanOrEqualTo({rule.Value})", rule.Message),
            _ => null
        };

    static string WithMessage(string ruleCall, string? message) =>
        message is not null
            ? $"{ruleCall}.WithMessage(\"{EscapeString(message)}\")"
            : ruleCall;

    static string EscapeString(string value) =>
        value.Replace("\\", "\\\\").Replace("\"", "\\\"");
}
