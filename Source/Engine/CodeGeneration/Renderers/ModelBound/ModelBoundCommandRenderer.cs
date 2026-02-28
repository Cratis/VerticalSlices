// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;
using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound;

/// <summary>
/// Renders a command descriptor as an Arc model-bound [Command] record with a Handle method.
/// </summary>
public class ModelBoundCommandRenderer : IArtifactRenderer<CommandDescriptor>
{
    /// <inheritdoc/>
    public IEnumerable<GeneratedFile> Render(CommandDescriptor descriptor, CodeGenerationContext context)
    {
        var producedEvents = descriptor.ProducedEvents.ToList();

        var usings = new List<string> { "Cratis.Arc.Commands.ModelBound", "Cratis.Chronicle.Events" };

        var allProperties = new Property[] { new(descriptor.EventSourceId, "EventSourceId") }
            .Concat(descriptor.Properties.Select(p => new Property(p.Name, p.Type)));

        var parameters = CodeWriter.FormatRecordParameters(allProperties);

        var builder = new StringBuilder()
            .AppendLine(CodeWriter.FormatUsings(usings))
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
            .AppendLine("[Command]")
            .AppendLine($"public record {descriptor.Name}({parameters})")
            .AppendLine("{");

        if (producedEvents.Count == 1)
        {
            var evt = producedEvents[0];
            builder
                .AppendLine("    /// <summary>")
                .AppendLine($"    /// Handles the {descriptor.Name} command.")
                .AppendLine("    /// </summary>")
                .AppendLine($"    /// <returns>The <see cref=\"{evt.Name}\"/> event to append.</returns>")
                .AppendLine($"    public {evt.Name} Handle()")
                .AppendLine("    {")
                .AppendLine("        // TODO: Implement command handling logic.")
                .AppendLine($"        // return new {evt.Name}(...);")
                .AppendLine("        throw new NotImplementedException();")
                .AppendLine("    }");
        }
        else if (producedEvents.Count > 1)
        {
            var eventStubs = string.Join(", ", producedEvents.Select(e => $"new {e.Name}(...)"));
            builder
                .AppendLine("    /// <summary>")
                .AppendLine($"    /// Handles the {descriptor.Name} command.")
                .AppendLine("    /// </summary>")
                .AppendLine("    /// <returns>The events to append.</returns>")
                .AppendLine("    public IEnumerable<object> Handle()")
                .AppendLine("    {")
                .AppendLine("        // TODO: Implement command handling logic.")
                .AppendLine($"        // return [{eventStubs}];")
                .AppendLine("        throw new NotImplementedException();")
                .AppendLine("    }");
        }
        else
        {
            builder
                .AppendLine("    /// <summary>")
                .AppendLine($"    /// Handles the {descriptor.Name} command.")
                .AppendLine("    /// </summary>")
                .AppendLine("    public void Handle()")
                .AppendLine("    {")
                .AppendLine("        // TODO: Implement command handling logic.")
                .AppendLine("    }");
        }

        builder.AppendLine("}");

        var relativePath = Path.Combine(context.RelativePath, $"{descriptor.Name}.cs");

        return [new GeneratedFile(relativePath, builder.ToString())];
    }
}
