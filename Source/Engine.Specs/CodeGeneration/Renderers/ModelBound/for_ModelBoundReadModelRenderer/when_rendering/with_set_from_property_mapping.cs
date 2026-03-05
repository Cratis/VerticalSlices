// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundReadModelRenderer.when_rendering;

public class with_set_from_property_mapping : given.a_context
{
    ModelBoundReadModelRenderer _renderer;
    ReadModelDescriptor _descriptor;
    string _projectionContent;

    void Establish()
    {
        _renderer = new ModelBoundReadModelRenderer();
        var mapping = new PropertyMapping("EmployeeRegistered", PropertyMappingKind.Set, "EmployeeId");
        _descriptor = new ReadModelDescriptor(
            "Employee",
            "An employee",
            [new ReadModelPropertyDescriptor("EmployeeId", "string", IsKey: true, [mapping])],
            []);
    }

    void Because() => _projectionContent = _renderer.Render(_descriptor, _context)
        .Single(f => f.ArtifactPath.EndsWith("Employee.cs")).Content;

    [Fact] void should_emit_from_event_attribute() => _projectionContent.ShouldContain("[FromEvent<EmployeeRegistered>]");
    [Fact] void should_not_emit_set_from_attribute() => _projectionContent.ShouldNotContain("[SetFrom<EmployeeRegistered>");
}
