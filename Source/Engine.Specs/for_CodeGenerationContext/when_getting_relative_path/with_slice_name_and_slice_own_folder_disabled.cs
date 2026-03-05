// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;

namespace Cratis.VerticalSlices.for_CodeGenerationContext.when_getting_relative_path;

public class with_slice_name_and_slice_own_folder_disabled : Specification
{
    CodeGenerationContext _context;
    string _result;

    void Establish() => _context = new CodeGenerationContext(
        "Orders",
        new FeaturePath(["Ordering"]),
        "PlaceOrder",
        new CodeGenerationOptions { SliceOwnFolder = false });

    void Because() => _result = _context.RelativePath;

    [Fact] void should_not_include_slice_name_in_path() =>
        _result.ShouldEqual(Path.Combine("Orders", "Ordering"));
}
