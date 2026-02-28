// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;
using Cratis.VerticalSlices.CodeGeneration.Output;

namespace Cratis.VerticalSlices.for_VerticalSlicesEngine.when_processing;

/// <summary>
/// Verifies that the engine writes concept files generated from feature-level concepts to the output.
/// </summary>
public class with_feature_level_concepts : given.all_dependencies
{
    ICodeOutput _output;
    VerticalSlicesEngine _engine;
    IEnumerable<Module> _modules;

    void Establish()
    {
        _output = Substitute.For<ICodeOutput>();
        _engine = new VerticalSlicesEngine(_codeGenerator, _logger);

        var concept = new Concept("EmployeeName", "string", "An employee name", []);
        var feature = new Feature("Registration", [concept], [], []);
        _modules = [new Module("HumanResources", [], [feature])];
    }

    async Task Because() => await _engine.Process(_modules, _output);

    [Fact] void should_write_concept_files_to_output() =>
        _output.Received(1).Write(Arg.Is<IEnumerable<GeneratedFile>>(files =>
            files.Any(f => f.RelativePath.EndsWith("EmployeeName.cs"))), Arg.Any<CancellationToken>());
}
