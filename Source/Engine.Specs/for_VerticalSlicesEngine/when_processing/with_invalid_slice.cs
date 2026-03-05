// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.EventModelAdvisory;

namespace Cratis.VerticalSlices.for_VerticalSlicesEngine.when_processing;

public class with_invalid_slice : given.all_dependencies
{
    VerticalSlicesEngine _engine;
    IEnumerable<Module> _modules;
    VerticalSlicesResult _result;

    void Establish()
    {
        var errorRecommendation = new EventModelRecommendation(
            EventModelRecommendationSeverity.Error,
            EventModelRecommendationCategory.Structure,
            "Mod",
            FeaturePath.Empty,
            "BadSlice",
            string.Empty,
            "A StateChange slice must have at least one command.");
        _advisor.Analyze(Arg.Any<IEnumerable<Module>>()).Returns([errorRecommendation]);
        _engine = new VerticalSlicesEngine(_codeGenerator, _advisor, _logger, _loggerFactory);
        var slice = new VerticalSlice("BadSlice", VerticalSliceType.StateChange, null, null, [], [], []);
        _modules = [new Module("Mod", [], [new Feature("Feat", [], [], [slice])])];
    }

    async Task Because() => _result = await _engine.Process(_modules);

    [Fact] void should_have_errors() => _result.HasErrors.ShouldBeTrue();
    [Fact] void should_return_no_artifacts() => _result.Artifacts.ShouldBeEmpty();
}
