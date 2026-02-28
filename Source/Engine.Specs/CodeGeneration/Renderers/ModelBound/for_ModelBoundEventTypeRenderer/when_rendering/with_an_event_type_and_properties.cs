// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundEventTypeRenderer.when_rendering;

public class with_an_event_type_and_properties : given.a_context
{
    ModelBoundEventTypeRenderer _renderer;
    EventTypeDescriptor _descriptor;
    IEnumerable<RenderedArtifact> _result;

    void Establish()
    {
        _renderer = new ModelBoundEventTypeRenderer();
        _descriptor = new EventTypeDescriptor(
            "OrderPlaced",
            "An order was placed",
            [new Property("OrderId", "Guid"), new Property("Amount", "decimal")]);
    }

    void Because() => _result = _renderer.Render(_descriptor, _context);

    [Fact] void should_return_one_file() => _result.Count().ShouldEqual(1);
    [Fact] void should_name_file_after_event_type() => _result.Single().ArtifactPath.EndsWith("OrderPlaced.cs").ShouldBeTrue();
    [Fact] void should_place_file_in_context_relative_path() => _result.Single().ArtifactPath.StartsWith("MyModule").ShouldBeTrue();
    [Fact] void should_emit_event_type_attribute() => _result.Single().Content.ShouldContain("[EventType]");
    [Fact] void should_emit_record_declaration() => _result.Single().Content.ShouldContain("public record OrderPlaced(");
    [Fact] void should_include_first_property() => _result.Single().Content.ShouldContain("Guid OrderId");
    [Fact] void should_include_second_property() => _result.Single().Content.ShouldContain("decimal Amount");
    [Fact] void should_declare_correct_namespace() => _result.Single().Content.ShouldContain("namespace MyModule.MyFeature.MySlice;");
}
