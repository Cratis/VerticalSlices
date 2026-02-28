// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundConceptRenderer.when_rendering;

/// <summary>
/// A bool concept has no meaningful "not set" sentinel, so no NotSet field should be emitted.
/// </summary>
public class with_bool_underlying_type : given.a_context
{
    ModelBoundConceptRenderer _renderer;
    ConceptDescriptor _descriptor;
    string _conceptContent;

    void Establish()
    {
        _renderer = new ModelBoundConceptRenderer();
        _descriptor = new ConceptDescriptor("FeatureFlag", "bool", "A feature flag toggle", []);
    }

    void Because() => _conceptContent = _renderer.Render(_descriptor, _context)
        .Single(f => f.RelativePath.EndsWith("FeatureFlag.cs")).Content;

    [Fact] void should_not_emit_not_set_field() =>
        _conceptContent.ShouldNotContain("NotSet");
}
