// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;
using Cratis.VerticalSlices.CodeGeneration.Renderers;

namespace Cratis.VerticalSlices.for_VerticalSlicesEngine.given;

public class a_module_with_a_slice_producing_files : all_dependencies
{
    protected IEnumerable<Module> _modules;
    protected RenderedArtifact _generatedFile;

    void Establish()
    {
        _generatedFile = new RenderedArtifact("Orders/Ordering/PlaceOrder/PlaceOrderCommand.cs", "// generated code");

        var internalEvent = new EventType("OrderPlaced", "An order was placed", [new Property("OrderId", "string")]);
        var readModel = new ReadModel("OrderList", "List of orders", []);
        var slice = new VerticalSlice(
            "PlaceOrder",
            VerticalSliceType.StateChange,
            null,
            null,
            [new Command("PlaceOrderCommand", "Places an order", [], "Id")],
            [readModel],
            [internalEvent]);

        var feature = new Feature("Ordering", [], [], [slice]);
        _modules = [new Module("Orders", [], [feature])];

        _codeGenerator
            .Generate(Arg.Any<VerticalSlice>(), Arg.Any<CodeGenerationContext>(), Arg.Any<ArtifactRenderSet>())
            .Returns([_generatedFile]);
    }
}
