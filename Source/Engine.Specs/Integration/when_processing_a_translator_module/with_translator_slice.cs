// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.Integration.when_processing_a_translator_module;

/// <summary>
/// End-to-end scenario: a Translator slice receives external events and produces internal events.
/// Only the internal event types should be generated as C# classes; external events are
/// structural and should NOT appear as generated files. The generated code must compile.
/// </summary>
public class with_translator_slice : given.a_real_engine
{
    IEnumerable<Module> _modules;

    void Establish()
    {
        // External input — not generated
        var externalEvent = new EventType(
            "ExternalOrderReceived",
            "An order received from the external system",
            [
                new Property("OrderRef", "string"),
                new Property("TotalAmount", "decimal")
            ],
            EventKind.External);

        // Internal output — must be generated
        var internalEvent = new EventType(
            "OrderIngested",
            "An external order was translated and ingested",
            [
                new Property("OrderId", "string"),
                new Property("Amount", "decimal")
            ],
            EventKind.Internal);

        var slice = new VerticalSlice(
            "OrderIngestion",
            VerticalSliceType.Translator,
            null,
            null,
            [],
            [],
            [externalEvent, internalEvent]);

        var feature = new Feature("OrderTranslation", [], [], [slice]);
        _modules = [new Module("Imports", [], [feature])];
    }

    async Task Because()
    {
        await _engine.Process(_modules, _output);
        _generatedFiles = _engine.Preview(_modules);
        AddGlobalUsingsFromGeneratedFiles();
        _buildExitCode = await RunDotnet("build");
    }

    [Fact] void should_generate_internal_event_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("OrderIngested.cs")).ShouldBeTrue();

    [Fact] void should_not_generate_external_event_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("ExternalOrderReceived.cs")).ShouldBeFalse();

    [Fact] void should_compile_successfully() => _buildExitCode.ShouldEqual(0);
}
