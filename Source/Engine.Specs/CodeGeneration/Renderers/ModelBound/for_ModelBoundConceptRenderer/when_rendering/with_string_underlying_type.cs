// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundConceptRenderer.when_rendering;

/// <summary>
/// When the underlying type is string, the concept should expose a static NotSet field
/// initialised to new(string.Empty).
/// </summary>
public class with_string_underlying_type : given.a_context
{
    ModelBoundConceptRenderer _renderer;
    ConceptDescriptor _descriptor;
    string _conceptContent;

    void Establish()
    {
        _renderer = new ModelBoundConceptRenderer();
        _descriptor = new ConceptDescriptor("ProductCode", "string", "A product code", []);
    }

    void Because() => _conceptContent = _renderer.Render(_descriptor, _context)
        .Single(f => f.ArtifactPath.EndsWith("ProductCode.cs")).Content;

    [Fact] void should_emit_not_set_field_with_empty_string() =>
        _conceptContent.ShouldContain("public static readonly ProductCode NotSet = new(string.Empty);");
}
