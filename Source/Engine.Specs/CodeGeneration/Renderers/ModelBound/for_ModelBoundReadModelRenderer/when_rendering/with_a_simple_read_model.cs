// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundReadModelRenderer.when_rendering;

public class with_a_simple_read_model : given.a_context
{
    ModelBoundReadModelRenderer _renderer;
    ReadModelDescriptor _descriptor;
    IEnumerable<GeneratedFile> _result;

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

    [Fact] void should_return_two_files() => _result.Count().ShouldEqual(2);
    [Fact] void should_generate_projection_file() => _result.Any(f => f.RelativePath.EndsWith("Employee.cs")).ShouldBeTrue();
    [Fact] void should_generate_observable_query_file() => _result.Any(f => f.RelativePath.EndsWith("AllEmployees.cs")).ShouldBeTrue();
    [Fact] void should_emit_key_attribute_on_first_property() => _result.Single(f => f.RelativePath.EndsWith("Employee.cs")).Content.ShouldContain("[Key]");
    [Fact] void should_emit_record_declaration() => _result.Single(f => f.RelativePath.EndsWith("Employee.cs")).Content.ShouldContain("public record Employee(");
    [Fact] void should_emit_read_model_attribute_on_query() => _result.Single(f => f.RelativePath.EndsWith("AllEmployees.cs")).Content.ShouldContain("[ReadModel]");
    [Fact] void should_emit_partial_record_for_query() => _result.Single(f => f.RelativePath.EndsWith("AllEmployees.cs")).Content.ShouldContain("public partial record AllEmployees");
    [Fact] void should_emit_get_all_method() => _result.Single(f => f.RelativePath.EndsWith("AllEmployees.cs")).Content.ShouldContain("GetAll(");
    [Fact] void should_declare_correct_namespace_in_projection() => _result.Single(f => f.RelativePath.EndsWith("Employee.cs")).Content.ShouldContain("namespace MyModule.MyFeature.MySlice;");
}
