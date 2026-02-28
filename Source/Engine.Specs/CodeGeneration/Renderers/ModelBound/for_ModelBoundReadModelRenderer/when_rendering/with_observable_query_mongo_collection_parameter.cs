// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundReadModelRenderer.when_rendering;

/// <summary>
/// The GetAll method on the observable query takes an IMongoCollection&lt;T&gt; parameter
/// where T matches the read model record type, wiring the query to MongoDB.
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
        .Single(f => f.RelativePath.EndsWith("AllTransactions.cs")).Content;

    [Fact] void should_take_imongo_collection_parameter() => _queryContent.ShouldContain("IMongoCollection<Transaction>");
    [Fact] void should_name_parameter_collection() => _queryContent.ShouldContain("collection)");
}
