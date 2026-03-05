// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.for_ReadModelDescriptor.when_creating_from_read_model;

public class with_unreferenced_events_available : Specification
{
    ReadModel _readModel;
    ReadModelDescriptor _result;

    void Establish()
    {
        var idProperty = new ReadModelProperty("Id", "string", []);
        _readModel = new ReadModel("OrderList", "List of orders", [idProperty]);
    }

    void Because() => _result = ReadModelDescriptor.FromReadModel(
        _readModel,
        [new EventType("UnrelatedEvent", "An unrelated event", [])]);

    [Fact] void should_not_include_unreferenced_events_in_source_events() => _result.SourceEvents.ShouldBeEmpty();
}
