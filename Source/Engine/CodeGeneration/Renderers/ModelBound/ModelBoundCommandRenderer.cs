// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound;

/// <summary>
/// Renders a command descriptor as an Arc model-bound [Command] record with a Handle method.
/// </summary>
public class ModelBoundCommandRenderer : IArtifactRenderer<CommandDescriptor>
{
    /// <inheritdoc/>
    public IEnumerable<RenderedArtifact> Render(CommandDescriptor descriptor, CodeGenerationContext context)
    {
        var producedEvents = descriptor.ProducedEvents.ToList();

        var allProperties = new Property[] { new(descriptor.EventSourceId, "EventSourceId") }
            .Concat(descriptor.Properties.Select(p => new Property(p.Name, p.Type)));

        var parameters = CodeWriter.FormatRecordParameters(allProperties);

        var builder = new CSharpCodeBuilder(context)
            .Using("Cratis.Arc.Commands.ModelBound", "Cratis.Chronicle.Events")
            .Namespace(context.Namespace)
            .BlankLine();

        if (!string.IsNullOrWhiteSpace(descriptor.Description))
        {
            builder.Summary(descriptor.Description);
        }

        builder
            .Attribute("Command")
            .OpenRecord(descriptor.Name, parameters);

        if (producedEvents.Count == 1)
        {
            var evt = producedEvents[0];
            var args = string.Join(", ", evt.Properties.Select(p => p.Name));

            builder
                .Summary($"Handles the {descriptor.Name} command.")
                .XmlReturns($"The <see cref=\"{evt.Name}\"/> event to append.")
                .ExpressionMember(evt.Name, "Handle", $"new {evt.Name}({args});");
        }
        else if (producedEvents.Count > 1)
        {
            var eventInstances = string.Join(", ", producedEvents.Select(e =>
            {
                var args = string.Join(", ", e.Properties.Select(p => p.Name));
                return $"new {e.Name}({args})";
            }));

            builder
                .Summary($"Handles the {descriptor.Name} command.")
                .XmlReturns("The events to append.")
                .ExpressionMember("IEnumerable<object>", "Handle", $"[{eventInstances}];");
        }
        else
        {
            builder
                .Summary($"Handles the {descriptor.Name} command.")
                .OpenMethod("void", "Handle")
                .EndBlock();
        }

        builder.EndBlock();

        var artifactPath = Path.Combine(context.RelativePath, $"{descriptor.Name}.cs");

        return [new RenderedArtifact(artifactPath, builder.ToString())];
    }
}
