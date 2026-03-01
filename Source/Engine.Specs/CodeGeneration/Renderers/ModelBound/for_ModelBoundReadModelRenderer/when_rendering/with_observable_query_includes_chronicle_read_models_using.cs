// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundReadModelRenderer.when_rendering;

/// <summary>
/// The read model file must include MongoDB.Driver and Cratis.Arc.MongoDB so that
/// IMongoCollection&lt;T&gt; and .Observe() resolve at compile time.
/// </summary>
public class with_observable_query_includes_chronicle_read_models_using : given.a_context
{
    ModelBoundReadModelRenderer _renderer;
    ReadModelDescriptor _descriptor;
    string _queryContent;

    void Establish()
    {
        _renderer = new ModelBoundReadModelRenderer();
        _descriptor = new ReadModelDescriptor(
            "Report",
            "A report",
            [new ReadModelPropertyDescriptor("ReportId", "string", IsKey: true, [])],
            []);
    }

    void Because() => _queryContent = _renderer.Render(_descriptor, _context)
        .Single(f => f.ArtifactPath.EndsWith("Report.cs")).Content;

    [Fact] void should_include_mongodb_driver_using() => _queryContent.ShouldContain("using MongoDB.Driver;");
    [Fact] void should_include_arc_mongodb_using() => _queryContent.ShouldContain("using Cratis.Arc.MongoDB;");
}
