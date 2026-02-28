// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.SliceTypes.for_AutomationCodeGenerator.when_generating;

public class with_events_read_models_and_commands : given.a_slice_type_code_generator
{
    AutomationCodeGenerator _generator;
    VerticalSlice _slice;
    RenderedArtifact _eventFile;
    RenderedArtifact _readModelFile;
    RenderedArtifact _commandFile;
    IEnumerable<RenderedArtifact> _result;

    void Establish()
    {
        _generator = new AutomationCodeGenerator();
        var domainEvent = new EventType("InvoiceReceived", "An invoice was received", []);
        var taskList = new ReadModel("PendingInvoices", "Invoices awaiting review", []);
        var command = new Command("ApproveInvoice", "Approves an invoice", [], "Id");
        _slice = new VerticalSlice("ReviewInvoice", VerticalSliceType.Automation, null, null, [command], [taskList], [domainEvent]);
        _eventFile = new RenderedArtifact("Test/InvoiceReceived.cs", "// event");
        _readModelFile = new RenderedArtifact("Test/PendingInvoices.cs", "// rm");
        _commandFile = new RenderedArtifact("Test/ApproveInvoice.cs", "// cmd");
        _eventTypeRenderer.Render(Arg.Any<EventTypeDescriptor>(), Arg.Any<CodeGenerationContext>()).Returns([_eventFile]);
        _readModelRenderer.Render(Arg.Any<ReadModelDescriptor>(), Arg.Any<CodeGenerationContext>()).Returns([_readModelFile]);
        _commandRenderer.Render(Arg.Any<CommandDescriptor>(), Arg.Any<CodeGenerationContext>()).Returns([_commandFile]);
    }

    void Because() => _result = _generator.Generate(_slice, _context, _renderSet);

    [Fact] void should_call_event_type_renderer() => _eventTypeRenderer.Received(1).Render(Arg.Any<EventTypeDescriptor>(), _context);
    [Fact] void should_call_read_model_renderer() => _readModelRenderer.Received(1).Render(Arg.Any<ReadModelDescriptor>(), _context);
    [Fact] void should_call_command_renderer() => _commandRenderer.Received(1).Render(Arg.Any<CommandDescriptor>(), _context);
    [Fact] void should_return_all_three_files() => _result.Count().ShouldEqual(3);
    [Fact] void should_include_event_file() => _result.ShouldContain(_eventFile);
    [Fact] void should_include_read_model_file() => _result.ShouldContain(_readModelFile);
    [Fact] void should_include_command_file() => _result.ShouldContain(_commandFile);
}
