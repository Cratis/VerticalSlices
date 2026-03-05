// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging.Abstractions;

namespace Cratis.VerticalSlices.CodeGeneration.Output.for_LocalFileSystemOutput.when_writing;

public class with_generated_files : Specification
{
    LocalFileSystemOutput _output;
    string _outputRoot;
    RenderedArtifact _file;

    void Establish()
    {
        _outputRoot = Path.Combine(Path.GetTempPath(), $"lfs_test_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_outputRoot);
        _output = new LocalFileSystemOutput(_outputRoot, NullLogger<LocalFileSystemOutput>.Instance);
        _file = new RenderedArtifact("PlaceOrder.cs", "// generated command");
    }

    async Task Because() => await _output.Write([_file]);

    [Fact] void should_write_the_file_to_disk() => File.Exists(Path.Combine(_outputRoot, "PlaceOrder.cs")).ShouldBeTrue();
    [Fact] void should_write_the_correct_content() => File.ReadAllText(Path.Combine(_outputRoot, "PlaceOrder.cs")).ShouldEqual("// generated command");
}
