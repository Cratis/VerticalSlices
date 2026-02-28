// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundReadModelRenderer.when_rendering;

/// <summary>
/// The GetAll method on the observable query must call Watch&lt;T&gt;().ToObservableReadModel()
/// on IReadModels to return a live reactive stream consumable by Chronicle's query infrastructure.
/// </summary>
public class with_observable_query_observe_method_call : given.a_context
{
    ModelBoundReadModelRenderer _renderer;
    ReadModelDescriptor _descriptor;
    string _queryContent;

    void Establish()
    {
        _renderer = new ModelBoundReadModelRenderer();
        _descriptor = new ReadModelDescriptor(
            "Shipment",
            "A shipment",
            [new ReadModelPropertyDescriptor("ShipmentId", "string", IsKey: true, [])],
            []);
    }

    void Because() => _queryContent = _renderer.Render(_descriptor, _context)
        .Single(f => f.ArtifactPath.EndsWith("Shipment.cs")).Content;

    [Fact] void should_call_watch_and_to_observable_read_model() => _queryContent.ShouldContain("Watch<Shipment>().ToObservableReadModel()");
    [Fact] void should_be_expression_bodied() => _queryContent.ShouldContain("=>");
}
