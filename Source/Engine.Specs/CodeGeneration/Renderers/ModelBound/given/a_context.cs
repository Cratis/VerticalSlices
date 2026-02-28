// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.Renderers.ModelBound.given;

/// <summary>
/// Base context providing a standard <see cref="CodeGenerationContext"/> for renderer specs.
/// </summary>
public class a_context : Specification
{
    protected CodeGenerationContext _context;

    void Establish() => _context = new CodeGenerationContext("MyModule", "MyFeature", ["MySlice"]);
}
