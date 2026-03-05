// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundReadModelRenderer.when_rendering;

/// <summary>
/// The observable query file generated for a read model must include the
/// Cratis.Arc.Queries.ModelBound namespace so that the [ReadModel] attribute resolves.
/// </summary>
public class with_observable_query_includes_arc_queries_using : given.a_context
{
    ModelBoundReadModelRenderer _renderer;
    ReadModelDescriptor _descriptor;
    string _queryContent;

    void Establish()
    {
        _renderer = new ModelBoundReadModelRenderer();
        _descriptor = new ReadModelDescriptor(
            "Notification",
            "A notification",
            [new ReadModelPropertyDescriptor("NotificationId", "string", IsKey: true, [])],
            []);
    }

    void Because() => _queryContent = _renderer.Render(_descriptor, _context)
        .Single(f => f.ArtifactPath.EndsWith("Notification.cs")).Content;

    [Fact] void should_include_arc_queries_model_bound_using() => _queryContent.ShouldContain("using Cratis.Arc.Queries.ModelBound;");
}
