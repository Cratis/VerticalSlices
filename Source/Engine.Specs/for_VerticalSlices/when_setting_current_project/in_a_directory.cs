// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json;

namespace Cratis.VerticalSlices.for_VerticalSlices.when_setting_current_project;

public class in_a_directory : Specification
{
    string _tempDirectory;
    string _projectFile;

    void Establish()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDirectory);
        _projectFile = "MyProject/MyProject.csproj";
    }

    void Because() => VerticalSlices.SetCurrentProject(_tempDirectory, _projectFile);

    void Cleanup()
    {
        if (Directory.Exists(_tempDirectory))
        {
            Directory.Delete(_tempDirectory, true);
        }
    }

    [Fact] void should_write_the_config_file() => File.Exists(Path.Combine(_tempDirectory, VerticalSlices.FileName)).ShouldBeTrue();
    [Fact] void should_persist_the_project_file_path() => JsonSerializer.Deserialize<VerticalSlices>(File.ReadAllText(Path.Combine(_tempDirectory, VerticalSlices.FileName)))!.ProjectFile.ShouldEqual(_projectFile);
}
