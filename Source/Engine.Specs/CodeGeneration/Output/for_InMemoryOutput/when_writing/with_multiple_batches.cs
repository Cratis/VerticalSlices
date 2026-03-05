// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.Output.for_InMemoryOutput.when_writing;

public class with_multiple_batches : Specification
{
    InMemoryOutput _output;
    RenderedArtifact _firstBatchFile;
    RenderedArtifact _secondBatchFile;

    void Establish()
    {
        _output = new InMemoryOutput();
        _firstBatchFile = new RenderedArtifact("First/File.cs", "// first");
        _secondBatchFile = new RenderedArtifact("Second/File.cs", "// second");
    }

    async Task Because()
    {
        await _output.Write([_firstBatchFile]);
        await _output.Write([_secondBatchFile]);
    }

    [Fact] void should_accumulate_files_from_all_batches() => _output.Artifacts.Count.ShouldEqual(2);
    [Fact] void should_contain_file_from_first_batch() => _output.Artifacts.ShouldContain(_firstBatchFile);
    [Fact] void should_contain_file_from_second_batch() => _output.Artifacts.ShouldContain(_secondBatchFile);
}
