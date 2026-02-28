// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundConceptRenderer.when_rendering;

/// <summary>
/// A DateOnly concept should expose a static NotSet field initialised to new(DateOnly.MinValue).
/// </summary>
public class with_dateonly_underlying_type : given.a_context
{
    ModelBoundConceptRenderer _renderer;
    ConceptDescriptor _descriptor;
    string _conceptContent;

    void Establish()
    {
        _renderer = new ModelBoundConceptRenderer();
        _descriptor = new ConceptDescriptor("EffectiveDate", "DateOnly", "An effective date", []);
    }

    void Because() => _conceptContent = _renderer.Render(_descriptor, _context)
        .Single(f => f.ArtifactPath.EndsWith("EffectiveDate.cs")).Content;

    [Fact] void should_emit_not_set_field_with_min_value() =>
        _conceptContent.ShouldContain("public static readonly EffectiveDate NotSet = new(DateOnly.MinValue);");
}
