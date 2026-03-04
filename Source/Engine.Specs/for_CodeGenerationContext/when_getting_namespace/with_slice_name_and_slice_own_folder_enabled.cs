// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;

namespace Cratis.VerticalSlices.for_CodeGenerationContext.when_getting_namespace;

public class with_slice_name_and_slice_own_folder_enabled : Specification
{
    CodeGenerationContext _context;
    string _result;

    void Establish() => _context = new CodeGenerationContext(
        "Orders",
        new FeaturePath(["Ordering"]),
        "PlaceOrder",
        new CodeGenerationOptions { SliceOwnFolder = true });

    void Because() => _result = _context.Namespace;

    [Fact] void should_append_slice_name_as_final_namespace_segment() =>
        _result.ShouldEqual("Orders.Ordering.PlaceOrder");
}
