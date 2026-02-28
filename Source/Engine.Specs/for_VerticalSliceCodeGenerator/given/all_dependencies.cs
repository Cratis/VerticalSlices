// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;
using Cratis.VerticalSlices.CodeGeneration.Renderers;
using Cratis.VerticalSlices.CodeGeneration.SliceTypes;
using Microsoft.Extensions.Logging;

namespace Cratis.VerticalSlices.for_VerticalSliceCodeGenerator.given;

public class all_dependencies : Specification
{
    protected ISliceTypeCodeGenerator _stateChangeGenerator;
    protected ILogger<VerticalSliceCodeGenerator> _logger;
    protected VerticalSliceCodeGenerator _generator;
    protected CodeGenerationContext _context;
    protected ArtifactRenderSet _renderSet;

    void Establish()
    {
        _stateChangeGenerator = Substitute.For<ISliceTypeCodeGenerator>();
        _stateChangeGenerator.SliceType.Returns(VerticalSliceType.StateChange);
        _logger = Substitute.For<ILogger<VerticalSliceCodeGenerator>>();
        _context = CodeGenerationContext.FromNamespace("TestNamespace");
        _renderSet = ArtifactRenderSet.ModelBound;
        _generator = new VerticalSliceCodeGenerator([_stateChangeGenerator], _logger);
    }
}
