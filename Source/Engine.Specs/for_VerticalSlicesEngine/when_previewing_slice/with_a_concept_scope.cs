// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;
using Cratis.VerticalSlices.CodeGeneration.Renderers;

namespace Cratis.VerticalSlices.for_VerticalSlicesEngine.when_previewing_slice;

/// <summary>
/// Verifies that a <see cref="ConceptScope"/> passed to <see cref="VerticalSlicesEngine.PreviewSlice"/>
/// is forwarded to the code generator via the <see cref="CodeGenerationContext"/>.
/// </summary>
public class with_a_concept_scope : given.all_dependencies
{
    VerticalSlicesEngine _engine;
    VerticalSlice _slice;
    ConceptScope _conceptScope;

    void Establish()
    {
        _engine = new VerticalSlicesEngine(_codeGenerator, _advisor, _logger, _loggerFactory);
        _slice = new VerticalSlice("RegisterEmployee", VerticalSliceType.StateChange, null, null, [], [], []);

        var concept = new Concept("EmployeeId", "Guid", "An employee identifier", []);
        _conceptScope = ConceptScope.Empty.With([concept], "HumanResources.Concepts");

        _codeGenerator
            .Generate(Arg.Any<VerticalSlice>(), Arg.Any<CodeGenerationContext>(), Arg.Any<ArtifactRenderSet>())
            .Returns([]);
    }

    void Because() => _engine.PreviewSlice(_slice, "HumanResources", new FeaturePath(["Registration"]), _conceptScope);

    [Fact]
    void should_pass_concept_scope_to_code_generator() =>
        _codeGenerator.Received(1).Generate(
            _slice,
            Arg.Is<CodeGenerationContext>(c => c.Concepts.IsConcept("EmployeeId")),
            Arg.Any<ArtifactRenderSet>());
}
