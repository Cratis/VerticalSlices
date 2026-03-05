// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.Output.for_InMemoryOutput.when_writing;

public class with_generated_files : Specification
{
    InMemoryOutput _output;
    RenderedArtifact _file1;
    RenderedArtifact _file2;

    void Establish()
    {
        _output = new InMemoryOutput();
        _file1 = new RenderedArtifact("Orders/PlaceOrder.cs", "// command");
        _file2 = new RenderedArtifact("Orders/OrderPlaced.cs", "// event");
    }

    async Task Because() => await _output.Write([_file1, _file2]);

    [Fact] void should_store_both_files() => _output.Artifacts.Count.ShouldEqual(2);
    [Fact] void should_contain_first_file() => _output.Artifacts.ShouldContain(_file1);
    [Fact] void should_contain_second_file() => _output.Artifacts.ShouldContain(_file2);
}
