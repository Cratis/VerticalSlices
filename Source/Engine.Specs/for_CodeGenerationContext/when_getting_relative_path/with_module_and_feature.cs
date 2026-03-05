// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;

namespace Cratis.VerticalSlices.for_CodeGenerationContext.when_getting_relative_path;

public class with_module_and_feature : Specification
{
    CodeGenerationContext _context;
    string _result;

    void Establish() => _context = new CodeGenerationContext("Orders", new FeaturePath(["Ordering"]), string.Empty);

    void Because() => _result = _context.RelativePath;

    [Fact] void should_combine_module_and_feature_using_path_separator() => _result.ShouldEqual(Path.Combine("Orders", "Ordering"));
}
