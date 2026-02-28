// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundReadModelRenderer.when_rendering;

/// <summary>
/// The observable query file for a read model must declare the property return type as
/// ISubject&lt;IEnumerable&lt;T&gt;&gt; so that Chronicle's reactive model works correctly.
/// </summary>
public class with_observable_query_uses_subject_of_ienumerable : given.a_context
{
    ModelBoundReadModelRenderer _renderer;
    ReadModelDescriptor _descriptor;
    string _queryContent;

    void Establish()
    {
        _renderer = new ModelBoundReadModelRenderer();
        _descriptor = new ReadModelDescriptor(
            "Invoice",
            "An invoice",
            [new ReadModelPropertyDescriptor("InvoiceId", "string", IsKey: true, [])],
            []);
    }

    void Because() => _queryContent = _renderer.Render(_descriptor, _context)
        .Single(f => f.RelativePath.EndsWith("AllInvoices.cs")).Content;

    [Fact] void should_use_isubject_return_type() => _queryContent.ShouldContain("ISubject<IEnumerable<Invoice>>");
    [Fact] void should_not_use_plain_ienumerable_return() => _queryContent.ShouldNotContain("IEnumerable<Invoice> GetAll");
}
