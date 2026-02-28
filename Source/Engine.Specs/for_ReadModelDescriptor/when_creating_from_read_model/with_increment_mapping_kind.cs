// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.for_ReadModelDescriptor.when_creating_from_read_model;

/// <summary>
/// An Increment mapping kind is preserved exactly through the descriptor creation.
/// Since Increment has no source property to read, the EventPropertyName is null.
/// </summary>
public class with_increment_mapping_kind : Specification
{
    ReadModel _readModel;
    ReadModelDescriptor _result;

    void Establish()
    {
        var incrementMapping = new EventPropertyMapping("TaskCompleted", EventPropertyMappingKind.Increment);

        _readModel = new ReadModel(
            "ProjectProgress",
            "Tracks project progress",
            [
                new ReadModelProperty("ProjectId", "string", [new EventPropertyMapping("TaskCompleted", EventPropertyMappingKind.Set, "ProjectId")]),
                new ReadModelProperty("TasksDone", "int", [incrementMapping])
            ]);
    }

    void Because() => _result = ReadModelDescriptor.FromReadModel(
        _readModel,
        [new EventType("TaskCompleted", "A task was completed", [new Property("ProjectId", "string")])]);

    [Fact] void should_preserve_increment_kind() =>
        _result.Properties.ElementAt(1).Mappings.First().Kind.ShouldEqual(PropertyMappingKind.Increment);

    [Fact] void should_have_null_event_property_name() =>
        _result.Properties.ElementAt(1).Mappings.First().EventPropertyName.ShouldBeNull();
}
