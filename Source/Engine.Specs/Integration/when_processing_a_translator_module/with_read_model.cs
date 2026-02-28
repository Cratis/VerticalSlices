// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.Integration.when_processing_a_translator_module;

/// <summary>
/// End-to-end scenario: a Translator slice that includes both external and internal events
/// AND a read model is fed to the engine. The external event must not produce a code file;
/// the internal event must produce a code file; and the read model must produce a projection
/// and an observable query. All generated files are compiled to verify correctness.
/// </summary>
public class with_read_model : given.a_real_engine
{
    IEnumerable<Module> _modules;

    void Establish()
    {
        var externalEvent = new EventType(
            "ExternalPaymentReceived",
            "A payment notification from an external payment provider",
            [
                new Property("PaymentId", "string"),
                new Property("Amount", "decimal")
            ],
            EventKind.External);

        var internalEvent = new EventType(
            "PaymentRecorded",
            "A payment was recorded in the internal ledger",
            [
                new Property("PaymentId", "string"),
                new Property("Amount", "decimal")
            ],
            EventKind.Internal);

        var paymentIdMappings = new[]
        {
            new EventPropertyMapping("PaymentRecorded", EventPropertyMappingKind.Set, "PaymentId")
        };

        var amountMappings = new[]
        {
            new EventPropertyMapping("PaymentRecorded", EventPropertyMappingKind.Set, "Amount")
        };

        var pendingPayment = new ReadModel(
            "RecordedPayment",
            "A payment that has been recorded",
            [
                new ReadModelProperty("PaymentId", "string", paymentIdMappings),
                new ReadModelProperty("Amount", "decimal", amountMappings)
            ]);

        var translatorSlice = new VerticalSlice(
            "TranslatePayment",
            VerticalSliceType.Translator,
            null,
            null,
            [],
            [pendingPayment],
            [externalEvent, internalEvent]);

        var feature = new Feature("Payments", [], [], [translatorSlice]);
        _modules = [new Module("Billing", [], [feature])];
    }

    async Task Because()
    {
        await _engine.Process(_modules, _output);
        _generatedFiles = _engine.Preview(_modules);
        AddGlobalUsingsFromGeneratedFiles();
        _buildExitCode = await RunDotnet("build");
    }

    [Fact] void should_not_generate_file_for_external_event() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("ExternalPaymentReceived.cs")).ShouldBeFalse();

    [Fact] void should_generate_file_for_internal_event() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("PaymentRecorded.cs")).ShouldBeTrue();

    [Fact] void should_generate_projection_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("RecordedPayment.cs")).ShouldBeTrue();

    [Fact] void should_generate_observable_query_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("AllRecordedPayments.cs")).ShouldBeTrue();

    [Fact] void should_compile_successfully() => _buildExitCode.ShouldEqual(0);
}
