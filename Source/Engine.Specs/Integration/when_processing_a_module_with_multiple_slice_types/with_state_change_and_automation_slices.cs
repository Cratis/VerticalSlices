// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.Integration.when_processing_a_module_with_multiple_slice_types;

/// <summary>
/// End-to-end scenario: a module with a StateChange slice and an Automation slice.
/// The Automation slice reacts to the event raised by the StateChange, maintains a task list,
/// and dispatches a follow-up command. All generated files must compile together.
/// </summary>
public class with_state_change_and_automation_slices : given.a_real_engine
{
    IEnumerable<Module> _modules;

    void Establish()
    {
        var invoiceReceived = new EventType(
            "InvoiceReceived",
            "An invoice was received from a supplier",
            [
                new Property("InvoiceId", "string"),
                new Property("SupplierId", "string"),
                new Property("Amount", "decimal")
            ]);

        var recordCommand = new Command(
            "RecordInvoice",
            "Records a new invoice",
            [
                new Property("SupplierId", "string"),
                new Property("Amount", "decimal")
            ],
            "InvoiceId");

        var stateChangeSlice = new VerticalSlice(
            "RecordInvoice",
            VerticalSliceType.StateChange,
            null,
            null,
            [recordCommand],
            [],
            [invoiceReceived]);

        var invoiceIdMappings = new[] { new EventPropertyMapping("InvoiceReceived", EventPropertyMappingKind.Set, "InvoiceId") };
        var supplierIdMappings = new[] { new EventPropertyMapping("InvoiceReceived", EventPropertyMappingKind.Set, "SupplierId") };
        var amountMappings = new[] { new EventPropertyMapping("InvoiceReceived", EventPropertyMappingKind.Set, "Amount") };

        var pendingInvoice = new ReadModel(
            "PendingInvoice",
            "An invoice awaiting approval",
            [
                new ReadModelProperty("InvoiceId", "string", invoiceIdMappings),
                new ReadModelProperty("SupplierId", "string", supplierIdMappings),
                new ReadModelProperty("Amount", "decimal", amountMappings)
            ]);

        var approveCommand = new Command(
            "ApproveInvoice",
            "Approves a pending invoice",
            [],
            "InvoiceId");

        var automationSlice = new VerticalSlice(
            "ApprovalAutomation",
            VerticalSliceType.Automation,
            null,
            null,
            [approveCommand],
            [pendingInvoice],
            [invoiceReceived]);

        var feature = new Feature("Invoicing", [], [], [stateChangeSlice, automationSlice]);
        _modules = [new Module("Finance", [], [feature])];
    }

    async Task Because()
    {
        await _engine.Process(_modules, _output);
        _generatedFiles = _engine.Preview(_modules);
        AddGlobalUsingsFromGeneratedFiles();
        _buildExitCode = await RunDotnet("build");
    }

    [Fact] void should_generate_event_type_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("InvoiceReceived.cs")).ShouldBeTrue();

    [Fact] void should_generate_state_change_command_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("RecordInvoice.cs")).ShouldBeTrue();

    [Fact] void should_generate_automation_read_model_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("PendingInvoice.cs")).ShouldBeTrue();

    [Fact] void should_generate_automation_command_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("ApproveInvoice.cs")).ShouldBeTrue();

    [Fact] void should_compile_successfully() => _buildExitCode.ShouldEqual(0);
}
