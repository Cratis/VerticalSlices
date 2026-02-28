// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;
using Microsoft.Extensions.Logging;

namespace Cratis.VerticalSlices.for_VerticalSlicesEngine.given;

public class all_dependencies : Specification
{
    protected IVerticalSliceCodeGenerator _codeGenerator;
    protected ILogger<VerticalSlicesEngine> _logger;

    void Establish()
    {
        _codeGenerator = Substitute.For<IVerticalSliceCodeGenerator>();
        _logger = Substitute.For<ILogger<VerticalSlicesEngine>>();
    }
}
