// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;
using Cratis.VerticalSlices.CodeGeneration.Output;
using Cratis.VerticalSlices.CodeGeneration.Renderers;

namespace Cratis.VerticalSlices.for_VerticalSlicesEngine.when_processing;

/// <summary>
/// When the engine is called with two independent modules, files from both modules must be
/// collected and written to the output. Neither module should suppress the other.
/// </summary>
public class with_multiple_modules : given.all_dependencies
{
    IEnumerable<Module> _modules;
    IEnumerable<GeneratedFile> _capturedFiles;
    ICodeOutput _output;
    VerticalSlicesEngine _engine;
    GeneratedFile _module1File;
    GeneratedFile _module2File;

    void Establish()
    {
        _module1File = new GeneratedFile("ModuleA/Feature1/Slice1/Event1.cs", "// m1");
        _module2File = new GeneratedFile("ModuleB/Feature2/Slice2/Event2.cs", "// m2");

        var event1 = new EventType("Event1", "Event 1", []);
        var event2 = new EventType("Event2", "Event 2", []);

        var slice1 = new VerticalSlice("Slice1", VerticalSliceType.StateChange, null, null, [], [], [event1]);
        var slice2 = new VerticalSlice("Slice2", VerticalSliceType.StateChange, null, null, [], [], [event2]);

        var feature1 = new Feature("Feature1", [], [], [slice1]);
        var feature2 = new Feature("Feature2", [], [], [slice2]);

        _modules = [new Module("ModuleA", [], [feature1]), new Module("ModuleB", [], [feature2])];

        _codeGenerator.Generate(
            Arg.Is<VerticalSlice>(s => s.Name == "Slice1"),
            Arg.Any<CodeGenerationContext>(),
            Arg.Any<ArtifactRenderSet>())
        .Returns([_module1File]);

        _codeGenerator.Generate(
            Arg.Is<VerticalSlice>(s => s.Name == "Slice2"),
            Arg.Any<CodeGenerationContext>(),
            Arg.Any<ArtifactRenderSet>())
        .Returns([_module2File]);

        _output = Substitute.For<ICodeOutput>();
        _output
            .Write(Arg.Do<IEnumerable<GeneratedFile>>(files => _capturedFiles = files.ToList()), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        _engine = new VerticalSlicesEngine(_codeGenerator, _logger);
    }

    async Task Because() => await _engine.Process(_modules, _output);

    [Fact] void should_include_file_from_first_module() => _capturedFiles.ShouldContain(_module1File);
    [Fact] void should_include_file_from_second_module() => _capturedFiles.ShouldContain(_module2File);
    [Fact] void should_write_two_files_in_total() => _capturedFiles.Count().ShouldEqual(2);
}
