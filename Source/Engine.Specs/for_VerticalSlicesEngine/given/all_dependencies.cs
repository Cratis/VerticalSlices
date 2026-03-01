// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.Chronicle;
using Cratis.VerticalSlices.CodeGeneration;
using Cratis.VerticalSlices.CodeGeneration.Output;
using Microsoft.Extensions.Logging;

namespace Cratis.VerticalSlices.for_VerticalSlicesEngine.given;

public class all_dependencies : Specification
{
    protected IVerticalSliceCodeGenerator _codeGenerator;
    protected ILogger<VerticalSlicesEngine> _logger;
    protected ICodeOutputResolver _outputResolver;
    protected IChronicleRegistrationResolver _chronicleResolver;

    void Establish()
    {
        _codeGenerator = Substitute.For<IVerticalSliceCodeGenerator>();
        _logger = Substitute.For<ILogger<VerticalSlicesEngine>>();
        _outputResolver = Substitute.For<ICodeOutputResolver>();
        _chronicleResolver = Substitute.For<IChronicleRegistrationResolver>();
        _outputResolver.Resolve().Returns(new NoOpCodeOutput());
        _chronicleResolver.Resolve().Returns(new NoOpChronicleRegistration());
    }
}
