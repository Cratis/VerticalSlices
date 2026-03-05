// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;
using Cratis.VerticalSlices.CodeGeneration.Renderers;

namespace Cratis.VerticalSlices.CodeGeneration.SliceTypes.given;

/// <summary>
/// Base context for slice type code generator specs, providing a mock <see cref="ArtifactRenderSet"/>
/// and a <see cref="CodeGenerationContext"/> ready for use.
/// </summary>
public class a_slice_type_code_generator : Specification
{
    protected IArtifactRenderer<EventTypeDescriptor> _eventTypeRenderer;
    protected IArtifactRenderer<CommandDescriptor> _commandRenderer;
    protected IArtifactRenderer<ReadModelDescriptor> _readModelRenderer;
    protected IArtifactRenderer<ConceptDescriptor> _conceptRenderer;
    protected ArtifactRenderSet _renderSet;
    protected CodeGenerationContext _context;

    void Establish()
    {
        _eventTypeRenderer = Substitute.For<IArtifactRenderer<EventTypeDescriptor>>();
        _commandRenderer = Substitute.For<IArtifactRenderer<CommandDescriptor>>();
        _readModelRenderer = Substitute.For<IArtifactRenderer<ReadModelDescriptor>>();
        _conceptRenderer = Substitute.For<IArtifactRenderer<ConceptDescriptor>>();
        _renderSet = new ArtifactRenderSet(_eventTypeRenderer, _commandRenderer, _readModelRenderer, _conceptRenderer);
        _context = CodeGenerationContext.FromNamespace("Test");
    }
}
