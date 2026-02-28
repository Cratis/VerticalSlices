// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.for_ReadModelDescriptor.when_creating_from_read_model;

/// <summary>
/// When the same property is mapped from two different event types (e.g. AccountOpened and
/// AccountRenamed both set the Name property), both events are deduplicated into the SourceEvents
/// and the property carries two mappings.
/// </summary>
public class with_same_property_mapped_from_two_events : Specification
{
    ReadModel _readModel;
    ReadModelDescriptor _result;

    void Establish()
    {
        var eventA = new EventType("AccountOpened", "Account opened", [new Property("AccountId", "string"), new Property("AccountName", "string")]);
        var eventB = new EventType("AccountRenamed", "Account renamed", [new Property("AccountId", "string"), new Property("NewName", "string")]);

        var mappingA = new EventPropertyMapping("AccountOpened", EventPropertyMappingKind.Set, "AccountName");
        var mappingB = new EventPropertyMapping("AccountRenamed", EventPropertyMappingKind.Set, "NewName");

        _readModel = new ReadModel(
            "AccountView",
            "A view of an account",
            [
                new ReadModelProperty("AccountId", "string", [new EventPropertyMapping("AccountOpened", EventPropertyMappingKind.Set, "AccountId")]),
                new ReadModelProperty("Name", "string", [mappingA, mappingB])
            ]);
    }

    void Because() => _result = ReadModelDescriptor.FromReadModel(
        _readModel,
        [
            new EventType("AccountOpened", "Account opened", [new Property("AccountId", "string"), new Property("AccountName", "string")]),
            new EventType("AccountRenamed", "Account renamed", [new Property("AccountId", "string"), new Property("NewName", "string")])
        ]);

    [Fact] void should_include_both_source_events() => _result.SourceEvents.Count().ShouldEqual(2);
    [Fact] void should_have_two_mappings_on_name_property() => _result.Properties.ElementAt(1).Mappings.Count().ShouldEqual(2);
    [Fact] void should_have_first_mapping_from_account_opened() => _result.Properties.ElementAt(1).Mappings.First().EventTypeName.ShouldEqual("AccountOpened");
    [Fact] void should_have_second_mapping_from_account_renamed() => _result.Properties.ElementAt(1).Mappings.ElementAt(1).EventTypeName.ShouldEqual("AccountRenamed");
}
