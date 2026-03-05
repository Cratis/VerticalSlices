// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundConceptRenderer.when_rendering;

/// <summary>
/// When a concept is marked as an event source id, it should inherit from <c>EventSourceId</c>
/// rather than <c>ConceptAs&lt;T&gt;</c>, use the Chronicle Events namespace, and always use
/// <see langword="string"/> as the underlying type regardless of what was specified.
/// </summary>
public class as_event_source_id : given.a_context
{
    ModelBoundConceptRenderer _renderer;
    ConceptDescriptor _descriptor;
    string _content;

    void Establish()
    {
        _renderer = new ModelBoundConceptRenderer();

        // Intentionally use Guid as underlying type to verify it is coerced to string.
        _descriptor = new ConceptDescriptor("PatientId", "Guid", "Uniquely identifies a patient", [], IsEventSourceId: true);
    }

    void Because() => _content = _renderer.Render(_descriptor, _context).Single().Content;

    [Fact] void should_inherit_from_event_source_id() => _content.ShouldContain(": EventSourceId(Value)");
    [Fact] void should_not_inherit_from_concept_as() => _content.ShouldNotContain("ConceptAs<");
    [Fact] void should_use_chronicle_events_namespace() => _content.ShouldContain("using Cratis.Chronicle.Events;");
    [Fact] void should_not_use_concepts_namespace() => _content.ShouldNotContain("using Cratis.Concepts;");
    [Fact] void should_always_use_string_as_underlying_type() => _content.ShouldContain("string Value");
    [Fact] void should_emit_implicit_operator_with_string() => _content.ShouldContain("public static implicit operator PatientId(string value) => new(Value: value);");
    [Fact] void should_emit_not_set_field_with_empty_string() => _content.ShouldContain("public static readonly PatientId NotSet = new(Value: string.Empty);");
}
