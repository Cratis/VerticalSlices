// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundReadModelRenderer.when_rendering;

public class with_set_from_context_mapping : given.a_context
{
    ModelBoundReadModelRenderer _renderer;
    ReadModelDescriptor _descriptor;
    string _projectionContent;

    void Establish()
    {
        _renderer = new ModelBoundReadModelRenderer();
        var mapping = new PropertyMapping("EmployeeRegistered", PropertyMappingKind.SetFromContext, ContextProperty: "Occurred");
        _descriptor = new ReadModelDescriptor(
            "Employee",
            "An employee",
            [new ReadModelPropertyDescriptor("RegisteredAt", "DateTimeOffset", IsKey: false, [mapping])],
            []);
    }

    void Because() => _projectionContent = _renderer.Render(_descriptor, _context)
        .Single(f => f.RelativePath.EndsWith("Employee.cs")).Content;

    [Fact] void should_emit_set_from_context_attribute() => _projectionContent.ShouldContain("[SetFromContext<EmployeeRegistered>(");
    [Fact] void should_reference_event_context_property_by_nameof() => _projectionContent.ShouldContain("nameof(EventContext.Occurred)");
}
