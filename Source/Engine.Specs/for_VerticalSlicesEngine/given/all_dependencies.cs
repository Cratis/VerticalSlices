// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;
using Cratis.VerticalSlices.EventModelAdvisory;
using Microsoft.Extensions.Logging;

namespace Cratis.VerticalSlices.for_VerticalSlicesEngine.given;

public class all_dependencies : Specification
{
    protected IVerticalSliceCodeGenerator _codeGenerator;
    protected IEventModelAdvisor _advisor;
    protected ILogger<VerticalSlicesEngine> _logger;
    protected ILoggerFactory _loggerFactory;

    void Establish()
    {
        _codeGenerator = Substitute.For<IVerticalSliceCodeGenerator>();
        _advisor = Substitute.For<IEventModelAdvisor>();
        _logger = Substitute.For<ILogger<VerticalSlicesEngine>>();
        _loggerFactory = Substitute.For<ILoggerFactory>();
        _advisor.Analyze(Arg.Any<IEnumerable<Module>>()).Returns([]);
    }
}


