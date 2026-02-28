// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundReadModelRenderer.when_rendering;

/// <summary>
/// The projection file must include `Cratis.Chronicle.Events` so that `EventContext` is
/// in scope for `nameof(EventContext.X)` expressions in [SetFromContext] and [FromEvery] attributes.
/// </summary>
public class with_projection_includes_chronicle_events_using : given.a_context
{
    ModelBoundReadModelRenderer _renderer;
    ReadModelDescriptor _descriptor;
    string _projectionContent;

    void Establish()
    {
        _renderer = new ModelBoundReadModelRenderer();
        _descriptor = new ReadModelDescriptor(
            "Ledger",
            "A ledger",
            [new ReadModelPropertyDescriptor("LedgerId", "string", IsKey: true, [])],
            []);
    }

    void Because() => _projectionContent = _renderer.Render(_descriptor, _context)
        .Single(f => f.RelativePath.EndsWith("Ledger.cs")).Content;

    [Fact] void should_include_chronicle_events_using() => _projectionContent.ShouldContain("using Cratis.Chronicle.Events;");
    [Fact] void should_include_chronicle_keys_using() => _projectionContent.ShouldContain("using Cratis.Chronicle.Keys;");
    [Fact] void should_include_model_bound_using() => _projectionContent.ShouldContain("using Cratis.Chronicle.Projections.ModelBound;");
}
