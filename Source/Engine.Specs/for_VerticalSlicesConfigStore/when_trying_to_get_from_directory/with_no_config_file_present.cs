// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.for_VerticalSlicesConfigStore.when_trying_to_get_from_directory;

public class with_no_config_file_present : Specification
{
    string _tempDirectory;
    bool _result;
    VerticalSlices _slices;

    void Establish() => _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

    void Because() => _result = VerticalSlicesConfigStore.TryGetFrom(_tempDirectory, out _slices!);

    void Cleanup()
    {
        if (Directory.Exists(_tempDirectory))
        {
            Directory.Delete(_tempDirectory, true);
        }
    }

    [Fact] void should_return_false() => _result.ShouldBeFalse();
    [Fact] void should_output_null_slices() => _slices.ShouldBeNull();
}
