// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundReadModelRenderer.when_rendering;

public class with_a_simple_read_model : given.a_context
{
    ModelBoundReadModelRenderer _renderer;
    ReadModelDescriptor _descriptor;
    IEnumerable<RenderedArtifact> _result;

    void Establish()
    {
        _renderer = new ModelBoundReadModelRenderer();
        _descriptor = new ReadModelDescriptor(
            "Employee",
            "An employee",
            [
                new ReadModelPropertyDescriptor("EmployeeId", "string", IsKey: true, []),
                new ReadModelPropertyDescriptor("Name", "string", IsKey: false, [])
            ],
            []);
    }

    void Because() => _result = _renderer.Render(_descriptor, _context);

    [Fact] void should_return_one_file() => _result.Count().ShouldEqual(1);
    [Fact] void should_generate_projection_file() => _result.Any(f => f.ArtifactPath.EndsWith("Employee.cs")).ShouldBeTrue();
    [Fact] void should_emit_key_attribute_on_first_property() => _result.Single(f => f.ArtifactPath.EndsWith("Employee.cs")).Content.ShouldContain("[Key]");
    [Fact] void should_emit_record_declaration() => _result.Single(f => f.ArtifactPath.EndsWith("Employee.cs")).Content.ShouldContain("public record Employee(");
    [Fact] void should_emit_read_model_attribute() => _result.Single(f => f.ArtifactPath.EndsWith("Employee.cs")).Content.ShouldContain("[ReadModel]");
    [Fact] void should_embed_all_query_method() => _result.Single(f => f.ArtifactPath.EndsWith("Employee.cs")).Content.ShouldContain("AllEmployees(");
    [Fact] void should_embed_by_id_query_method() => _result.Single(f => f.ArtifactPath.EndsWith("Employee.cs")).Content.ShouldContain("EmployeeById(");
    [Fact] void should_declare_correct_namespace_in_projection() => _result.Single(f => f.ArtifactPath.EndsWith("Employee.cs")).Content.ShouldContain("namespace MyModule.MyFeature;");
}
