// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundReadModelRenderer.when_rendering;

public class with_every_event_mapping : given.a_context
{
    ModelBoundReadModelRenderer _renderer;
    ReadModelDescriptor _descriptor;
    string _projectionContent;

    void Establish()
    {
        _renderer = new ModelBoundReadModelRenderer();

        // A SetFromContext mapping with EventTypeName == "*" triggers a [FromEvery] attribute.
        var everyMapping = new PropertyMapping("*", PropertyMappingKind.SetFromContext, ContextProperty: "Occurred");
        _descriptor = new ReadModelDescriptor(
            "Employee",
            "An employee",
            [
                new ReadModelPropertyDescriptor("EmployeeId", "string", IsKey: true, []),
                new ReadModelPropertyDescriptor("LastUpdated", "DateTimeOffset", IsKey: false, [everyMapping])
            ],
            []);
    }

    void Because() => _projectionContent = _renderer.Render(_descriptor, _context)
        .Single(f => f.ArtifactPath.EndsWith("Employee.cs")).Content;

    [Fact] void should_emit_from_every_attribute() => _projectionContent.ShouldContain("[FromEvery(contextProperty:");
    [Fact] void should_reference_occurred_context_property() => _projectionContent.ShouldContain("nameof(EventContext.Occurred)");
}
