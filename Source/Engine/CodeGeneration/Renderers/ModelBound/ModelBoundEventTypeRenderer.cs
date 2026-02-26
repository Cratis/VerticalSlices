// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;
using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound;

/// <summary>
/// Renders an event type descriptor as a Chronicle [EventType] record.
/// </summary>
public class ModelBoundEventTypeRenderer : IArtifactRenderer<EventTypeDescriptor>
{
    /// <inheritdoc/>
    public IEnumerable<GeneratedFile> Render(EventTypeDescriptor descriptor, CodeGenerationContext context)
    {
        var parameters = CodeWriter.FormatRecordParameters(descriptor.Properties);

        var builder = new StringBuilder()
            .AppendLine(CodeWriter.FormatUsings(["Cratis.Chronicle.Events"]))
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
            .AppendLine("[EventType]")
            .Append($"public record {descriptor.Name}({parameters});")
            .AppendLine();

        var relativePath = Path.Combine(context.RelativePath, $"{descriptor.Name}.cs");

        return [new GeneratedFile(relativePath, builder.ToString())];
    }
}
