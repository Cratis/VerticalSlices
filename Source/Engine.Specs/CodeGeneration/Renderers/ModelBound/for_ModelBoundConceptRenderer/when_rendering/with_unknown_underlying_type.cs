// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundConceptRenderer.when_rendering;

/// <summary>
/// A concept whose underlying CLR type is not explicitly handled (e.g. Uri) should
/// not emit a NotSet sentinel, because no suitable default expression exists.
/// </summary>
public class with_unknown_underlying_type : given.a_context
{
    ModelBoundConceptRenderer _renderer;
    ConceptDescriptor _descriptor;
    string _conceptContent;

    void Establish()
    {
        _renderer = new ModelBoundConceptRenderer();
        _descriptor = new ConceptDescriptor("ResourceLocator", "Uri", "A resource URI", []);
    }

    void Because() => _conceptContent = _renderer.Render(_descriptor, _context)
        .Single(f => f.ArtifactPath.EndsWith("ResourceLocator.cs")).Content;

    [Fact] void should_not_emit_not_set_field() =>
        _conceptContent.ShouldNotContain("NotSet");
}
