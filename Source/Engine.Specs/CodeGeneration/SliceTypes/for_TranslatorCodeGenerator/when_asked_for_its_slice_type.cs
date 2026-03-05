// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration.SliceTypes.for_TranslatorCodeGenerator;

public class when_asked_for_its_slice_type : Specification
{
    TranslatorCodeGenerator _generator;
    VerticalSliceType _result;

    void Establish() => _generator = new TranslatorCodeGenerator();

    void Because() => _result = _generator.SliceType;

    [Fact] void should_return_translator() => _result.ShouldEqual(VerticalSliceType.Translator);
}
