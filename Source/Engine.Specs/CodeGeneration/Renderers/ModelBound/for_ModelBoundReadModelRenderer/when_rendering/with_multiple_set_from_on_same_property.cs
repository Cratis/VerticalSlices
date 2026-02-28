// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.for_ModelBoundReadModelRenderer.when_rendering;

/// <summary>
/// A property can be populated by multiple different events. When two SetFrom mappings target the
/// same property from different event types, the renderer must emit a [SetFrom&lt;T&gt;] attribute
/// for each event type — the read model property is mapped from whichever event fires last.
/// </summary>
public class with_multiple_set_from_on_same_property : given.a_context
{
    ModelBoundReadModelRenderer _renderer;
    ReadModelDescriptor _descriptor;
    string _projectionContent;

    void Establish()
    {
        _renderer = new ModelBoundReadModelRenderer();

        var mappingFromCreated = new PropertyMapping("AccountCreated", PropertyMappingKind.Set, "AccountName");
        var mappingFromRenamed = new PropertyMapping("AccountRenamed", PropertyMappingKind.Set, "NewName");

        _descriptor = new ReadModelDescriptor(
            "AccountView",
            "View of an account",
            [
                new ReadModelPropertyDescriptor("AccountId", "string", IsKey: true, []),
                new ReadModelPropertyDescriptor("Name", "string", IsKey: false, [mappingFromCreated, mappingFromRenamed])
            ],
            []);
    }

    void Because() => _projectionContent = _renderer.Render(_descriptor, _context)
        .Single(f => f.RelativePath.EndsWith("AccountView.cs")).Content;

    [Fact] void should_emit_set_from_created_attribute() => _projectionContent.ShouldContain("[SetFrom<AccountCreated>");
    [Fact] void should_emit_set_from_renamed_attribute() => _projectionContent.ShouldContain("[SetFrom<AccountRenamed>");
    [Fact] void should_reference_account_name_property() => _projectionContent.ShouldContain("nameof(AccountCreated.AccountName)");
    [Fact] void should_reference_new_name_property() => _projectionContent.ShouldContain("nameof(AccountRenamed.NewName)");
}
