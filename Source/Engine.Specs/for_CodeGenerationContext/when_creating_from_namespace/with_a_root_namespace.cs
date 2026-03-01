// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;

namespace Cratis.VerticalSlices.for_CodeGenerationContext.when_creating_from_namespace;

public class with_a_root_namespace : Specification
{
    CodeGenerationContext _result;

    void Because() => _result = CodeGenerationContext.FromNamespace("Cratis.Orders");

    [Fact] void should_set_namespace_to_provided_value() => _result.Namespace.ShouldEqual("Cratis.Orders");
    [Fact] void should_have_empty_feature_path() => _result.FeaturePath.ShouldEqual(FeaturePath.Empty);
    [Fact] void should_have_empty_slice_name() => _result.SliceName.ShouldEqual(string.Empty);
}
