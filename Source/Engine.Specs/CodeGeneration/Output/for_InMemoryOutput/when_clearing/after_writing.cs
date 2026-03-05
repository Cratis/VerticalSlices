// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.Output.for_InMemoryOutput.when_clearing;

public class after_writing : Specification
{
    InMemoryOutput _output;

    void Establish() => _output = new InMemoryOutput();

    async Task Because()
    {
        await _output.Write([new RenderedArtifact("Orders/PlaceOrder.cs", "// code")]);
        _output.Clear();
    }

    [Fact] void should_have_no_files() => _output.Artifacts.ShouldBeEmpty();
}
