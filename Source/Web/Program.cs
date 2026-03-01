// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json;
using Cratis.VerticalSlices;
using Cratis.VerticalSlices.Chronicle;
using Cratis.VerticalSlices.CodeGeneration;
using Cratis.VerticalSlices.CodeGeneration.Output;
using Cratis.VerticalSlices.CodeGeneration.SliceTypes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ISliceTypeCodeGenerator, StateChangeCodeGenerator>();
builder.Services.AddSingleton<ISliceTypeCodeGenerator, StateViewCodeGenerator>();
builder.Services.AddSingleton<ISliceTypeCodeGenerator, AutomationCodeGenerator>();
builder.Services.AddSingleton<ISliceTypeCodeGenerator, TranslatorCodeGenerator>();
builder.Services.AddSingleton<IVerticalSliceCodeGenerator, VerticalSliceCodeGenerator>();
builder.Services.AddSingleton<ICodeOutputResolver>(provider =>
{
    var outputRoot = builder.Configuration["VerticalSlices:OutputRoot"] ?? "./generated";
    var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
    var output = new LocalFileSystemOutput(outputRoot, loggerFactory.CreateLogger<LocalFileSystemOutput>());
    return new CodeOutputResolver(output);
});
builder.Services.AddSingleton<IChronicleRegistrationResolver, ChronicleRegistrationResolver>();
builder.Services.AddSingleton<IVerticalSlicesEngine, VerticalSlicesEngine>();

var app = builder.Build();

var structureFile = app.Configuration["VerticalSlices:StructureFile"] ?? "vertical-slices-structure.json";
if (File.Exists(structureFile))
{
    var json = await File.ReadAllTextAsync(structureFile);
    var modules = JsonSerializer.Deserialize<IEnumerable<Module>>(json, JsonSerializerOptions.Web) ?? [];

    var engine = app.Services.GetRequiredService<IVerticalSlicesEngine>();
    await engine.Process(modules);
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
    var files = engine.Preview(modules);

    return Results.Ok(files);
});

await app.RunAsync();
