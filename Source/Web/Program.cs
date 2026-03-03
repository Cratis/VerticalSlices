// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json;
using Cratis.VerticalSlices;
using Cratis.VerticalSlices.CodeGeneration.Output;

var builder = WebApplication.CreateBuilder(args);

var outputRoot = builder.Configuration["VerticalSlices:OutputRoot"] ?? "./generated";
var outputOptions = new CodeOutputOptions { Target = CodeOutputTarget.LocalFileSystem, OutputRoot = outputRoot };

builder.Services.AddVerticalSlices();

var app = builder.Build();

var structureFile = app.Configuration["VerticalSlices:StructureFile"] ?? "vertical-slices-structure.json";
if (File.Exists(structureFile))
{
    var json = await File.ReadAllTextAsync(structureFile);
    var modules = JsonSerializer.Deserialize<IEnumerable<Module>>(json, JsonSerializerOptions.Web) ?? [];

    var engine = app.Services.GetRequiredService<IVerticalSlicesEngine>();
    await engine.Process(modules, outputOptions: outputOptions);
}

app.MapGet("/", () => "VerticalSlices Engine is running.");

app.MapGet("/preview", (IVerticalSlicesEngine engine) =>
{
    var structurePath = app.Configuration["VerticalSlices:StructureFile"] ?? "vertical-slices-structure.json";
    if (!File.Exists(structurePath))
    {
        return Results.NotFound("No vertical slices structure file found.");
    }

    var json = File.ReadAllText(structurePath);
    var modules = JsonSerializer.Deserialize<IEnumerable<Module>>(json, JsonSerializerOptions.Web) ?? [];
    var result = engine.Preview(modules);

    return Results.Ok(result);
});

await app.RunAsync();
