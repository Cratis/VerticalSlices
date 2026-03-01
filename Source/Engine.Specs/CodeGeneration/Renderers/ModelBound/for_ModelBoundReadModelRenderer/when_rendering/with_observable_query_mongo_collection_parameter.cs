// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundReadModelRenderer.when_rendering;

/// <summary>
/// The query methods on the read model take an IMongoCollection&lt;T&gt; parameter
/// so MongoDB can observe changes to the collection.
/// </summary>
public class with_observable_query_mongo_collection_parameter : given.a_context
{
    ModelBoundReadModelRenderer _renderer;
    ReadModelDescriptor _descriptor;
    string _queryContent;

    void Establish()
    {
        _renderer = new ModelBoundReadModelRenderer();
        _descriptor = new ReadModelDescriptor(
            "Transaction",
            "A financial transaction",
            [new ReadModelPropertyDescriptor("TransactionId", "string", IsKey: true, [])],
            []);
    }

    void Because() => _queryContent = _renderer.Render(_descriptor, _context)
        .Single(f => f.ArtifactPath.EndsWith("Transaction.cs")).Content;

    [Fact] void should_take_imongo_collection_parameter() => _queryContent.ShouldContain("IMongoCollection<Transaction> collection");
    [Fact] void should_not_use_iread_models() => _queryContent.ShouldNotContain("IReadModels");
}
