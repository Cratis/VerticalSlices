// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundReadModelRenderer.when_rendering;

/// <summary>
/// When a property carries a SetFromContext mapping with EventTypeName == "*" (i.e. a FromEvery
/// mapping), the renderer must emit only [FromEvery(contextProperty: ...)] and must NOT emit
/// [SetFromContext&lt;*&gt;(...)] since "*" is not a valid C# type name.
/// </summary>
public class with_every_event_mapping_does_not_emit_set_from_context : given.a_context
{
    ModelBoundReadModelRenderer _renderer;
    ReadModelDescriptor _descriptor;
    string _projectionContent;

    void Establish()
    {
        _renderer = new ModelBoundReadModelRenderer();

        var everyMapping = new PropertyMapping("*", PropertyMappingKind.SetFromContext, ContextProperty: "Occurred");

        _descriptor = new ReadModelDescriptor(
            "AuditRecord",
            "An audit record",
            [
                new ReadModelPropertyDescriptor("Id", "string", IsKey: true, []),
                new ReadModelPropertyDescriptor("LastSeen", "DateTimeOffset", IsKey: false, [everyMapping])
            ],
            []);
    }

    void Because() => _projectionContent = _renderer.Render(_descriptor, _context)
        .Single(f => f.RelativePath.EndsWith("AuditRecord.cs")).Content;

    [Fact] void should_emit_from_every_attribute() => _projectionContent.ShouldContain("[FromEvery(contextProperty: nameof(EventContext.Occurred))]");
    [Fact] void should_not_emit_set_from_context_with_wildcard() => _projectionContent.ShouldNotContain("[SetFromContext<*>");
}
