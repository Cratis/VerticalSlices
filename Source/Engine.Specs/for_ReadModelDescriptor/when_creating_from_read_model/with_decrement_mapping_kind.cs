// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.for_ReadModelDescriptor.when_creating_from_read_model;

/// <summary>
/// A Decrement mapping kind is preserved through the descriptor creation with no source property.
/// Useful for modelling reversals, e.g. reducing a completed-task count when a task is reopened.
/// </summary>
public class with_decrement_mapping_kind : Specification
{
    ReadModel _readModel;
    ReadModelDescriptor _result;

    void Establish()
    {
        var decrementMapping = new EventPropertyMapping("TaskReopened", EventPropertyMappingKind.Decrement);

        _readModel = new ReadModel(
            "ProjectProgress",
            "Tracks project progress",
            [
                new ReadModelProperty("ProjectId", "string", [new EventPropertyMapping("TaskReopened", EventPropertyMappingKind.Set, "ProjectId")]),
                new ReadModelProperty("TasksDone", "int", [decrementMapping])
            ]);
    }

    void Because() => _result = ReadModelDescriptor.FromReadModel(
        _readModel,
        [new EventType("TaskReopened", "A task was reopened", [new Property("ProjectId", "string")])]);

    [Fact] void should_preserve_decrement_kind() =>
        _result.Properties.ElementAt(1).Mappings.First().Kind.ShouldEqual(PropertyMappingKind.Decrement);

    [Fact] void should_have_null_event_property_name() =>
        _result.Properties.ElementAt(1).Mappings.First().EventPropertyName.ShouldBeNull();
}
