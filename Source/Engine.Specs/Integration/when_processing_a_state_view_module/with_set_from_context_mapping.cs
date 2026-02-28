// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.Integration.when_processing_a_state_view_module;

/// <summary>
/// End-to-end scenario: a read model property is populated from the EventContext.Occurred
/// timestamp via a SetFromContext mapping. The generated projection must compile — this
/// requires that Cratis.Chronicle.Events is in scope for nameof(EventContext.Occurred).
/// </summary>
public class with_set_from_context_mapping : given.a_real_engine
{
    IEnumerable<Module> _modules;

    void Establish()
    {
        var documentSaved = new EventType(
            "DocumentSaved",
            "A document was saved",
            [
                new Property("DocumentId", "string"),
                new Property("AuthorId", "string")
            ]);

        // StateChange slice generates the event type class
        var saveCommand = new Command(
            "SaveDocument",
            "Saves a document",
            [new Property("AuthorId", "string")],
            "DocumentId");

        var stateChangeSlice = new VerticalSlice(
            "DocumentManagement",
            VerticalSliceType.StateChange,
            null,
            null,
            [saveCommand],
            [],
            [documentSaved]);

        var docFeature = new Feature("Documents", [], [], [stateChangeSlice]);

        // Read model: document summary with context-sourced LastModified
        var docIdMappings = new[]
        {
            new EventPropertyMapping("DocumentSaved", EventPropertyMappingKind.Set, "DocumentId")
        };

        var authorMappings = new[]
        {
            new EventPropertyMapping("DocumentSaved", EventPropertyMappingKind.Set, "AuthorId")
        };

        var lastModifiedMappings = new[]
        {
            // SetFromContext maps from the event's metadata rather than a payload property
            new EventPropertyMapping("DocumentSaved", EventPropertyMappingKind.SetFromContext, "Occurred")
        };

        var docSummary = new ReadModel(
            "DocumentSummary",
            "Summary of a document",
            [
                new ReadModelProperty("DocumentId", "string", docIdMappings),
                new ReadModelProperty("AuthorId", "string", authorMappings),
                new ReadModelProperty("LastModified", "DateTimeOffset", lastModifiedMappings)
            ]);

        var stateViewSlice = new VerticalSlice(
            "DocumentView",
            VerticalSliceType.StateView,
            null,
            null,
            [],
            [docSummary],
            [documentSaved]);

        var viewFeature = new Feature("DocumentView", [], [], [stateViewSlice]);

        _modules = [new Module("ContentManagement", [], [docFeature, viewFeature])];
    }

    async Task Because()
    {
        await _engine.Process(_modules, _output);
        _generatedFiles = _engine.Preview(_modules);
        AddGlobalUsingsFromGeneratedFiles();
        _buildExitCode = await RunDotnet("build");
    }

    [Fact] void should_generate_projection_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("DocumentSummary.cs")).ShouldBeTrue();

    [Fact] void should_generate_observable_query_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("AllDocumentSummarys.cs")).ShouldBeTrue();

    [Fact] void should_compile_successfully() => _buildExitCode.ShouldEqual(0);
}
