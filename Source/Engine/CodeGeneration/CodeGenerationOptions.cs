// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.CodeGeneration;

/// <summary>
/// Options that control how the code generator emits source files.
/// </summary>
public record CodeGenerationOptions
{
    /// <summary>
    /// Gets a value indicating whether to emit all using directives as <c>global using</c>
    /// declarations in a single generated file rather than as per-file using directives.
    /// When <see langword="false"/> (the default), each generated source file contains its own using directives.
    /// </summary>
    public bool UseGlobalUsings { get; init; }

    /// <summary>
    /// Gets the root namespace prefix for all generated code.
    /// When set, the generated namespace becomes <c>{RootNamespace}.{Module}.{Feature}...</c>.
    /// When empty, the module name is the root of the namespace.
    /// </summary>
    public string RootNamespace { get; init; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether each slice gets its own subfolder inside the feature folder,
    /// named after the slice. When <see langword="true"/> (the default), a slice named <c>PlaceOrder</c>
    /// inside the <c>Ordering</c> feature produces files under <c>Module/Ordering/PlaceOrder/</c>.
    /// When <see langword="false"/>, all slice files are written directly into the feature folder.
    /// </summary>
    public bool SliceOwnFolder { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether all artifacts for a single slice should be emitted
    /// in a single file rather than one file per artifact type.
    /// Defaults to <see langword="true"/>.
    /// </summary>
    public bool SingleFilePerSlice { get; init; } = true;

    /// <summary>
    /// Gets the rendering style for projections.
    /// Determines whether read models use model-bound attributes, declarative <c>IProjectionFor</c>,
    /// or reducer-based projection classes.
    /// Defaults to <see cref="ProjectionStyle.ModelBound"/>.
    /// </summary>
    public ProjectionStyle ProjectionStyle { get; init; } = ProjectionStyle.ModelBound;

    /// <summary>
    /// Gets the rendering style for commands.
    /// Determines whether commands use model-bound <c>[Command]</c> records with <c>Handle()</c>
    /// or produce a separate controller.
    /// Defaults to <see cref="CommandStyle.ModelBound"/>.
    /// </summary>
    public CommandStyle CommandStyle { get; init; } = CommandStyle.ModelBound;

    /// <summary>
    /// Gets the rendering style for read model query endpoints.
    /// Determines whether queries use the model-bound <c>[ReadModel]</c> pattern
    /// or produce a separate <c>[ApiController]</c>.
    /// Defaults to <see cref="ReadModelEndpointStyle.ModelBound"/>.
    /// </summary>
    public ReadModelEndpointStyle ReadModelEndpointStyle { get; init; } = ReadModelEndpointStyle.ModelBound;

    /// <summary>
    /// Gets the number of spaces per indentation level.
    /// Defaults to 4. Ignored when <see cref="UseTabs"/> is <see langword="true"/>.
    /// </summary>
    public int IndentSize { get; init; } = 4;

    /// <summary>
    /// Gets a value indicating whether to use tab characters for indentation instead of spaces.
    /// When <see langword="true"/>, <see cref="IndentSize"/> is ignored and a single tab is used per level.
    /// Defaults to <see langword="false"/>.
    /// </summary>
    public bool UseTabs { get; init; }
}
