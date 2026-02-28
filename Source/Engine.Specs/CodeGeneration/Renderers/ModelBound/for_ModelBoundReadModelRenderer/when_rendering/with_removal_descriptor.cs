// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundReadModelRenderer.when_rendering;

public class with_removal_descriptor : given.a_context
{
    ModelBoundReadModelRenderer _renderer;
    ReadModelDescriptor _descriptor;
    string _projectionContent;

    void Establish()
    {
        _renderer = new ModelBoundReadModelRenderer();
        _descriptor = new ReadModelDescriptor(
            "Employee",
            "An employee",
            [new ReadModelPropertyDescriptor("EmployeeId", "string", IsKey: true, [])],
            [],
            Removal: new RemovalDescriptor("EmployeeDeleted"));
    }

    void Because() => _projectionContent = _renderer.Render(_descriptor, _context)
        .Single(f => f.RelativePath.EndsWith("Employee.cs")).Content;

    [Fact] void should_emit_removed_with_attribute() => _projectionContent.ShouldContain("[RemovedWith<EmployeeDeleted>]");
}
