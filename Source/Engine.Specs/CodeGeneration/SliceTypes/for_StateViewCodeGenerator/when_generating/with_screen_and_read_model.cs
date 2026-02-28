// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.CodeGeneration.SliceTypes.for_StateViewCodeGenerator.when_generating;

/// <summary>
/// When a StateView slice carries a Screen, the Screen must be forwarded to
/// ReadModelDescriptor.FromReadModel so that matching field labels are resolved.
/// The spec captures the ReadModelDescriptor passed to the renderer and asserts that
/// the matching property carries the FieldType from the screen.
/// </summary>
public class with_screen_and_read_model : given.a_slice_type_code_generator
{
    StateViewCodeGenerator _generator;
    VerticalSlice _slice;
    ReadModelDescriptor _capturedDescriptor;

    void Establish()
    {
        _generator = new StateViewCodeGenerator();

        var screen = new Screen(
            "EmployeeForm",
            "Employee form",
            [new ScreenField("Name", "string", ScreenFieldType.TextInput, "Full Name")]);

        var nameMapping = new EventPropertyMapping("EmployeeRegistered", EventPropertyMappingKind.Set, "FullName");
        var readModel = new ReadModel(
            "Employee",
            "An employee projection",
            [new ReadModelProperty("Name", "string", [nameMapping])]);

        _slice = new VerticalSlice(
            "Employees",
            VerticalSliceType.StateView,
            null,
            screen,
            [],
            [readModel],
            [new EventType("EmployeeRegistered", "Employee registered", [new Property("FullName", "string")])]);

        _readModelRenderer
            .Render(Arg.Do<ReadModelDescriptor>(d => _capturedDescriptor = d), Arg.Any<CodeGenerationContext>())
            .Returns([]);
    }

    void Because() => _generator.Generate(_slice, _context, _renderSet);

    [Fact] void should_pass_screen_field_type_to_descriptor() =>
        _capturedDescriptor.Properties.First().FieldType.ShouldEqual(ScreenFieldType.TextInput);
}
