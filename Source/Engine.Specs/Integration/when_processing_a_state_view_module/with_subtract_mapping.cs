// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.Integration.when_processing_a_state_view_module;

/// <summary>
/// End-to-end scenario: a read model uses [SubtractFrom&lt;T&gt;] to reduce a balance
/// when a debit event fires. The generated projection and observable query must compile.
/// </summary>
public class with_subtract_mapping : given.a_real_engine
{
    IEnumerable<Module> _modules;

    void Establish()
    {
        var accountDebited = new EventType(
            "AccountDebited",
            "Money was debited from the account",
            [
                new Property("AccountId", "string"),
                new Property("Amount", "decimal")
            ]);

        var accountIdMappings = new[]
        {
            new EventPropertyMapping("AccountDebited", EventPropertyMappingKind.Set, "AccountId")
        };

        var balanceMappings = new[]
        {
            new EventPropertyMapping("AccountDebited", EventPropertyMappingKind.Subtract, "Amount")
        };

        var accountView = new ReadModel(
            "AccountView",
            "A view of an account balance",
            [
                new ReadModelProperty("AccountId", "string", accountIdMappings),
                new ReadModelProperty("Balance", "decimal", balanceMappings)
            ]);

        var stateChangeSlice = new VerticalSlice(
            "Debit",
            VerticalSliceType.StateChange,
            null,
            null,
            [],
            [],
            [accountDebited]);

        var stateViewSlice = new VerticalSlice(
            "AccountView",
            VerticalSliceType.StateView,
            null,
            null,
            [],
            [accountView],
            [accountDebited]);

        var feature = new Feature("Accounts", [], [], [stateChangeSlice, stateViewSlice]);
        _modules = [new Module("Banking", [], [feature])];
    }

    async Task Because()
    {
        await _engine.Process(_modules, _output);
        _generatedFiles = _engine.Preview(_modules);
        AddGlobalUsingsFromGeneratedFiles();
        _buildExitCode = await RunDotnet("build");
    }

    [Fact] void should_generate_projection_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("AccountView.cs")).ShouldBeTrue();

    [Fact] void should_generate_observable_query_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("AllAccountViews.cs")).ShouldBeTrue();

    [Fact] void should_compile_successfully() => _buildExitCode.ShouldEqual(0);
}
