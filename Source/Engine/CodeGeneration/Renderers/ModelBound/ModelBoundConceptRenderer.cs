// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound;

/// <summary>
/// Renders a concept descriptor as a ConceptAs record and, when validation rules are present,
/// a ConceptValidator class — both combined into a single <c>{ConceptName}.cs</c> file.
/// </summary>
public class ModelBoundConceptRenderer : IArtifactRenderer<ConceptDescriptor>
{
    /// <inheritdoc/>
    public IEnumerable<RenderedArtifact> Render(ConceptDescriptor descriptor, CodeGenerationContext context)
    {
        var conceptArtifact = RenderConcept(descriptor, context);

        if (!descriptor.HasValidation)
        {
            return [conceptArtifact];
        }

        var validatorArtifact = RenderValidator(descriptor, context);
        var compositionContext = context with { SliceName = descriptor.Name };

        return [SliceFileComposer.Compose([conceptArtifact, validatorArtifact], compositionContext)];
    }

    static RenderedArtifact RenderConcept(ConceptDescriptor descriptor, CodeGenerationContext context)
    {
        var usingNs = descriptor.IsEventSourceId ? "Cratis.Chronicle.Events" : "Cratis.Concepts";

        // EventSourceId is always string-backed in Chronicle regardless of what was specified.
        var underlyingType = descriptor.IsEventSourceId ? "string" : descriptor.UnderlyingType;

        var baseType = descriptor.IsEventSourceId
            ? "EventSourceId(Value)"
            : $"ConceptAs<{descriptor.UnderlyingType}>(Value)";

        var builder = new CSharpCodeBuilder(context)
            .Using(usingNs)
            .Namespace(context.Namespace)
            .BlankLine();

        if (!string.IsNullOrWhiteSpace(descriptor.Description))
        {
            builder.Summary(descriptor.Description);
        }

        builder
            .OpenRecord(descriptor.Name, $"{underlyingType} Value", baseType)
            .Statement($"public static implicit operator {descriptor.Name}({underlyingType} value) => new(Value: value);");

        AppendStaticDefaults(builder, descriptor, underlyingType);

        builder.EndBlock();

        var artifactPath = Path.Combine(context.RelativePath, $"{descriptor.Name}.cs");

        return new RenderedArtifact(artifactPath, builder.ToString());
    }

    static RenderedArtifact RenderValidator(ConceptDescriptor descriptor, CodeGenerationContext context)
    {
        var builder = new CSharpCodeBuilder(context)
            .Using("Cratis.Arc.Validation", "FluentValidation")
            .Namespace(context.Namespace)
            .BlankLine()
            .Summary($"Validates the <see cref=\"{descriptor.Name}\"/> concept.")
            .OpenClass($"{descriptor.Name}Validator", $"ConceptValidator<{descriptor.Name}>")
            .Summary($"Initializes a new instance of the <see cref=\"{descriptor.Name}Validator\"/> class.")
            .OpenConstructor($"{descriptor.Name}Validator");

        foreach (var rule in descriptor.ValidationRules)
        {
            var ruleExpression = FormatValidationRule(rule);
            if (ruleExpression is not null)
            {
                builder.Statement($"RuleFor(x => x.Value){ruleExpression};");
            }
        }

        builder
            .EndBlock()
            .EndBlock();

        var artifactPath = Path.Combine(context.RelativePath, $"{descriptor.Name}Validator.cs");

        return new RenderedArtifact(artifactPath, builder.ToString());
    }

    static void AppendStaticDefaults(CSharpCodeBuilder builder, ConceptDescriptor descriptor, string underlyingType)
    {
        var defaultExpression = underlyingType switch
        {
            "string" => "new(Value: string.Empty)",
            "Guid" => "new(Value: Guid.Empty)",
            "int" or "long" or "short" or "byte"
                or "uint" or "ulong" or "ushort" or "sbyte"
                or "float" or "double" or "decimal" => "new(Value: 0)",
            "bool" => null,
            "DateTime" => "new(Value: DateTime.MinValue)",
            "DateOnly" => "new(Value: DateOnly.MinValue)",
            "TimeOnly" => "new(Value: TimeOnly.MinValue)",
            "DateTimeOffset" => "new(Value: DateTimeOffset.MinValue)",
            _ => null
        };

        if (defaultExpression is not null)
        {
            builder
                .BlankLine()
                .Statement($"public static readonly {descriptor.Name} NotSet = {defaultExpression};");
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
