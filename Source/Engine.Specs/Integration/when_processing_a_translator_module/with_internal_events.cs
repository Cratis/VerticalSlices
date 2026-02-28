// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.Integration.when_processing_a_translator_module;

/// <summary>
/// End-to-end scenario: a Translator slice with both external (input) and internal (output)
/// events is fed to the engine. Only internal events should generate code files.
/// The generated C# files are written to disk and then compiled with <c>dotnet build</c>.
/// </summary>
public class with_internal_events : given.a_real_engine
{
    IEnumerable<Module> _modules;

    void Establish()
    {
        var legacyEvent = new EventType(
            "LegacyOrderCreated",
            "A legacy order created event from an external system",
            [
                new Property("OrderId", "string"),
                new Property("CustomerName", "string")
            ],
            EventKind.External);

        var internalEvent = new EventType(
            "OrderCreated",
            "An order was created",
            [
                new Property("OrderId", "string"),
                new Property("CustomerName", "string")
            ],
            EventKind.Internal);

        var translatorSlice = new VerticalSlice(
            "TranslateLegacyOrder",
            VerticalSliceType.Translator,
            null,
            null,
            [],
            [],
            [legacyEvent, internalEvent]);

        var feature = new Feature("OrderTranslation", [], [], [translatorSlice]);
        _modules = [new Module("OrderManagement", [], [feature])];
    }

    async Task Because()
    {
        await _engine.Process(_modules, _output);
        _generatedFiles = _engine.Preview(_modules);
        _buildExitCode = await RunDotnet("build");
    }

    [Fact] void should_not_generate_file_for_external_event() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("LegacyOrderCreated.cs")).ShouldBeFalse();

    [Fact] void should_generate_file_for_internal_event() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("OrderCreated.cs")).ShouldBeTrue();

    [Fact] void should_compile_successfully() => _buildExitCode.ShouldEqual(0);
}
