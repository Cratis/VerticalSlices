// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json;

namespace Cratis.VerticalSlices.for_VerticalSlicesConfigStore.when_trying_to_get_from_directory;

public class with_config_file_present : Specification
{
    string _tempDirectory;
    bool _result;
    VerticalSlices _slices;

    void Establish()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDirectory);
        var json = JsonSerializer.Serialize(new { ProjectFile = "MyProject/MyProject.csproj" });
        File.WriteAllText(Path.Combine(_tempDirectory, VerticalSlices.FileName), json);
    }

    void Because() => _result = VerticalSlicesConfigStore.TryGetFrom(_tempDirectory, out _slices!);

    void Cleanup()
    {
        if (Directory.Exists(_tempDirectory))
        {
            Directory.Delete(_tempDirectory, true);
        }
    }

    [Fact] void should_return_true() => _result.ShouldBeTrue();
    [Fact] void should_output_loaded_slices() => _slices.ShouldNotBeNull();
    [Fact] void should_populate_project_file() => _slices.ProjectFile.ShouldEqual("MyProject/MyProject.csproj");
}
