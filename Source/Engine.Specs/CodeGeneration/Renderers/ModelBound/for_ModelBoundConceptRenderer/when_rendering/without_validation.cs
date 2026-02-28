// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundConceptRenderer.when_rendering;

public class without_validation : given.a_context
{
    ModelBoundConceptRenderer _renderer;
    ConceptDescriptor _descriptor;
    IEnumerable<RenderedArtifact> _result;

    void Establish()
    {
        _renderer = new ModelBoundConceptRenderer();
        _descriptor = new ConceptDescriptor("OrderId", "string", "Identifies an order", []);
    }

    void Because() => _result = _renderer.Render(_descriptor, CodeGenerationContext.FromNamespace("MyModule"));

    [Fact] void should_return_one_file() => _result.Count().ShouldEqual(1);
    [Fact] void should_name_file_after_concept() => _result.Single().ArtifactPath.EndsWith("OrderId.cs").ShouldBeTrue();
    [Fact] void should_emit_concept_record_declaration() => _result.Single().Content.ShouldContain("public record OrderId(string Value) : ConceptAs<string>(Value)");
    [Fact] void should_emit_implicit_conversion_operator() => _result.Single().Content.ShouldContain("public static implicit operator OrderId(string value) => new(value);");
    [Fact] void should_emit_not_set_default() => _result.Single().Content.ShouldContain("public static readonly OrderId NotSet = new(string.Empty);");
}
