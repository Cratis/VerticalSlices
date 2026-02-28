// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound;

/// <summary>
/// Renders an event type descriptor as a Chronicle [EventType] record.
/// </summary>
public class ModelBoundEventTypeRenderer : IArtifactRenderer<EventTypeDescriptor>
{
    /// <inheritdoc/>
    public IEnumerable<RenderedArtifact> Render(EventTypeDescriptor descriptor, CodeGenerationContext context)
    {
        var parameters = CodeWriter.FormatRecordParameters(descriptor.Properties);

        var builder = new CSharpCodeBuilder(context)
            .Using("Cratis.Chronicle.Events")
            .Namespace(context.Namespace)
            .BlankLine();

        if (!string.IsNullOrWhiteSpace(descriptor.Description))
        {
            builder.Summary(descriptor.Description);
        }

        builder
            .Attribute("EventType")
            .Record(descriptor.Name, parameters);

        var artifactPath = Path.Combine(context.RelativePath, $"{descriptor.Name}.cs");

        return [new RenderedArtifact(artifactPath, builder.ToString())];
    }
}
