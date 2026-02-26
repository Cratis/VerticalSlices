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
        var parameters = CodeWriter.FormatRecordParameters(
            descriptor.Properties.Select(p => new Property(p.Name, p.Type)));

        var producedEvents = descriptor.ProducedEvents.ToList();

        var usings = new List<string> { "Cratis.Arc.Commands.ModelBound" };
        if (producedEvents.Count > 0)
        {
            usings.Add("Cratis.Chronicle.Events");
        }

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

        if (producedEvents.Count > 0)
        {
            builder
                .AppendLine("    /// <summary>")
                .AppendLine($"    /// Handles the {descriptor.Name} command.")
                .AppendLine("    /// </summary>")
                .AppendLine("    /// <param name=\"eventLog\">The event log to append events to.</param>")
                .AppendLine("    /// <returns>A <see cref=\"Task\"/> representing the asynchronous operation.</returns>")
                .AppendLine("    public Task Handle(IEventLog eventLog)")
                .AppendLine("    {")
                .AppendLine("        // TODO: Implement command handling logic.")
                .AppendLine("        // Append events to the event log, for example:");

            foreach (var evt in producedEvents)
            {
                builder.AppendLine($"        // await eventLog.Append(id, new {evt.Name}(...));");
            }

            builder
                .AppendLine("        return Task.CompletedTask;")
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
