// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.for_ReadModelDescriptor.when_creating_from_read_model;

/// <summary>
/// A Set mapping kind reads a named property from an event and places it onto the read model.
/// The source property name must be preserved in EventPropertyName.
/// </summary>
public class with_set_mapping_kind : Specification
{
    ReadModel _readModel;
    ReadModelDescriptor _result;

    void Establish()
    {
        var setMapping = new EventPropertyMapping("ProductCreated", EventPropertyMappingKind.Set, "ProductName");

        _readModel = new ReadModel(
            "ProductView",
            "A view of a product",
            [new ReadModelProperty("Name", "string", [setMapping])]);
    }

    void Because() => _result = ReadModelDescriptor.FromReadModel(
        _readModel,
        [new EventType("ProductCreated", "Product created", [new Property("ProductName", "string")])]);

    [Fact] void should_preserve_set_kind() =>
        _result.Properties.First().Mappings.First().Kind.ShouldEqual(PropertyMappingKind.Set);

    [Fact] void should_route_source_property_to_event_property_name() =>
        _result.Properties.First().Mappings.First().EventPropertyName.ShouldEqual("ProductName");

    [Fact] void should_have_null_context_property() =>
        _result.Properties.First().Mappings.First().ContextProperty.ShouldBeNull();
}
