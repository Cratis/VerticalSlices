// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.Integration.when_processing_a_state_view_module;

/// <summary>
/// End-to-end scenario: a read model property is populated by [FromEvery] — it maps from
/// the EventContext.Occurred timestamp regardless of which event fires. The EventTypeName "*"
/// triggers [FromEvery] instead of [SetFromContext&lt;T&gt;]. The generated code must compile.
/// </summary>
public class with_from_every_context_mapping : given.a_real_engine
{
    IEnumerable<Module> _modules;

    void Establish()
    {
        var blogPostPublished = new EventType(
            "BlogPostPublished",
            "A blog post was published",
            [
                new Property("PostId", "string"),
                new Property("Title", "string")
            ]);

        // StateChange so the event type class is generated
        var publishCommand = new Command(
            "PublishBlogPost",
            "Publishes a blog post",
            [new Property("Title", "string")],
            "PostId");

        var stateChangeSlice = new VerticalSlice(
            "Publishing",
            VerticalSliceType.StateChange,
            null,
            null,
            [publishCommand],
            [],
            [blogPostPublished]);

        var publishingFeature = new Feature("Publishing", [], [], [stateChangeSlice]);

        // Read model: blog post with a LastModified property using FromEvery
        var postIdMappings = new[]
        {
            new EventPropertyMapping("BlogPostPublished", EventPropertyMappingKind.Set, "PostId")
        };

        var titleMappings = new[]
        {
            new EventPropertyMapping("BlogPostPublished", EventPropertyMappingKind.Set, "Title")
        };

        // "*" as EventTypeName + SetFromContext → renders [FromEvery(contextProperty: ...)]
        var lastModifiedMappings = new[]
        {
            new EventPropertyMapping("*", EventPropertyMappingKind.SetFromContext, "Occurred")
        };

        var blogPostView = new ReadModel(
            "BlogPostView",
            "A view of a blog post",
            [
                new ReadModelProperty("PostId", "string", postIdMappings),
                new ReadModelProperty("Title", "string", titleMappings),
                new ReadModelProperty("LastModified", "DateTimeOffset", lastModifiedMappings)
            ]);

        var stateViewSlice = new VerticalSlice(
            "BlogView",
            VerticalSliceType.StateView,
            null,
            null,
            [],
            [blogPostView],
            [blogPostPublished]);

        var viewFeature = new Feature("BlogView", [], [], [stateViewSlice]);

        _modules = [new Module("Blog", [], [publishingFeature, viewFeature])];
    }

    async Task Because()
    {
        await _engine.Process(_modules, _output);
        _generatedFiles = _engine.Preview(_modules);
        AddGlobalUsingsFromGeneratedFiles();
        _buildExitCode = await RunDotnet("build");
    }

    [Fact] void should_generate_projection_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("BlogPostView.cs")).ShouldBeTrue();

    [Fact] void should_generate_observable_query_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("AllBlogPostViews.cs")).ShouldBeTrue();

    [Fact] void should_compile_successfully() => _buildExitCode.ShouldEqual(0);
}
