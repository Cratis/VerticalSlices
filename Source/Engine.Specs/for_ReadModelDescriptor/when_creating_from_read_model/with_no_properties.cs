// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.for_ReadModelDescriptor.when_creating_from_read_model;

public class with_no_properties : Specification
{
    ReadModel _readModel;
    ReadModelDescriptor _result;

    void Establish() => _readModel = new ReadModel("EmptyView", "An empty view", []);

    void Because() => _result = ReadModelDescriptor.FromReadModel(_readModel, []);

    [Fact] void should_map_name() => _result.Name.ShouldEqual("EmptyView");
    [Fact] void should_map_description() => _result.Description.ShouldEqual("An empty view");
    [Fact] void should_have_no_properties() => _result.Properties.ShouldBeEmpty();
    [Fact] void should_have_no_source_events() => _result.SourceEvents.ShouldBeEmpty();
}
