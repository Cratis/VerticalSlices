// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging.Abstractions;

namespace Cratis.VerticalSlices.CodeGeneration.Output.for_LocalFileSystemOutput.when_writing;

public class with_nested_path : Specification
{
    LocalFileSystemOutput _output;
    string _outputRoot;
    GeneratedFile _file;

    void Establish()
    {
        _outputRoot = Path.Combine(Path.GetTempPath(), $"lfs_test_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_outputRoot);
        _output = new LocalFileSystemOutput(_outputRoot, NullLogger<LocalFileSystemOutput>.Instance);
        _file = new GeneratedFile(Path.Combine("Orders", "Placing", "PlaceOrder.cs"), "// command");
    }

    async Task Because() => await _output.Write([_file]);

    [Fact] void should_create_intermediate_directories() =>
        Directory.Exists(Path.Combine(_outputRoot, "Orders", "Placing")).ShouldBeTrue();

    [Fact] void should_write_the_file_at_the_nested_path() =>
        File.Exists(Path.Combine(_outputRoot, "Orders", "Placing", "PlaceOrder.cs")).ShouldBeTrue();
}
