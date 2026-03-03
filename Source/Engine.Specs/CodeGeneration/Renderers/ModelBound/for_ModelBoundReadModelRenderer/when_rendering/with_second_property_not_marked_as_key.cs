// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundReadModelRenderer.when_rendering;

/// <summary>
/// Only the first property in a read model receives the [Key] attribute. All subsequent
/// properties — even if they might conceptually be identifiers — must not carry [Key].
/// </summary>
public class with_second_property_not_marked_as_key : given.a_context
{
    ModelBoundReadModelRenderer _renderer;
    ReadModelDescriptor _descriptor;
    string _projectionContent;

    void Establish()
    {
        _renderer = new ModelBoundReadModelRenderer();
        _descriptor = new ReadModelDescriptor(
            "Product",
            "A product",
            [
                new ReadModelPropertyDescriptor("ProductId", "string", IsKey: true, []),
                new ReadModelPropertyDescriptor("Sku", "string", IsKey: false, []),
                new ReadModelPropertyDescriptor("Name", "string", IsKey: false, [])
            ],
            []);
    }

    void Because() => _projectionContent = _renderer.Render(_descriptor, _context)
        .Single(f => f.ArtifactPath.EndsWith("Product.cs")).Content;

    [Fact] void should_emit_key_attribute_exactly_once() =>
        _projectionContent
            .Split('\n')
            .Count(l => l.TrimStart().StartsWith("[Key]"))
            .ShouldEqual(1);

    [Fact] void should_emit_key_attribute_before_first_property() =>
        _projectionContent.IndexOf("[Key]").ShouldBeLessThan(_projectionContent.IndexOf("ProductId"));

    [Fact] void should_emit_all_three_properties() =>
        new[] { "ProductId", "Sku", "Name" }.All(_projectionContent.Contains).ShouldBeTrue();
}
