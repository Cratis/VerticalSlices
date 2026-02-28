// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;

namespace Cratis.VerticalSlices.for_CodeGenerationContext.when_getting_namespace;

public class with_module_feature_and_sub_features : Specification
{
    CodeGenerationContext _context;
    string _result;

    void Establish() => _context = new CodeGenerationContext("Orders", "Ordering", ["PlaceOrder", "Validation"]);

    void Because() => _result = _context.Namespace;

    [Fact] void should_combine_all_parts_with_dots() => _result.ShouldEqual("Orders.Ordering.PlaceOrder.Validation");
}
