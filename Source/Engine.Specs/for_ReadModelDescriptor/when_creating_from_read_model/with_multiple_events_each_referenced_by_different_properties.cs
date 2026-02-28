// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.for_ReadModelDescriptor.when_creating_from_read_model;

/// <summary>
/// When two properties each reference a different event type, both events must appear in
/// the SourceEvents collection of the resulting descriptor.
/// </summary>
public class with_multiple_events_each_referenced_by_different_properties : Specification
{
    ReadModel _readModel;
    EventType _firstEvent;
    EventType _secondEvent;
    ReadModelDescriptor _result;

    void Establish()
    {
        _firstEvent = new EventType("ProductCreated", "A product was created", [new Property("ProductId", "string"), new Property("Name", "string")]);
        _secondEvent = new EventType("ProductPriceUpdated", "Price updated", [new Property("ProductId", "string"), new Property("Price", "decimal")]);

        var idMapping = new EventPropertyMapping("ProductCreated", EventPropertyMappingKind.Set, "ProductId");
        var nameMapping = new EventPropertyMapping("ProductCreated", EventPropertyMappingKind.Set, "Name");
        var priceMapping = new EventPropertyMapping("ProductPriceUpdated", EventPropertyMappingKind.Set, "Price");

        _readModel = new ReadModel(
            "ProductCatalogEntry",
            "Entry in the product catalog",
            [
                new ReadModelProperty("ProductId", "string", [idMapping]),
                new ReadModelProperty("Name", "string", [nameMapping]),
                new ReadModelProperty("Price", "decimal", [priceMapping])
            ]);
    }

    void Because() => _result = ReadModelDescriptor.FromReadModel(_readModel, [_firstEvent, _secondEvent]);

    [Fact] void should_include_both_events_in_source_events() => _result.SourceEvents.Count().ShouldEqual(2);
    [Fact] void should_include_product_created_event() => _result.SourceEvents.Any(e => e.Name == "ProductCreated").ShouldBeTrue();
    [Fact] void should_include_price_updated_event() => _result.SourceEvents.Any(e => e.Name == "ProductPriceUpdated").ShouldBeTrue();
    [Fact] void should_have_three_properties() => _result.Properties.Count().ShouldEqual(3);
    [Fact] void should_have_two_mappings_on_first_property() => _result.Properties.First().Mappings.Count().ShouldEqual(1);
}
