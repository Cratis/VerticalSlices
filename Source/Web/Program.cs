// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json;
using Cratis.VerticalSlices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IVerticalSlicesEngine, VerticalSlicesEngine>();

var app = builder.Build();

var structureFile = app.Configuration["VerticalSlices:StructureFile"] ?? "vertical-slices-structure.json";
if (File.Exists(structureFile))
{
    var json = await File.ReadAllTextAsync(structureFile);
    var features = JsonSerializer.Deserialize<IEnumerable<Feature>>(json, JsonSerializerOptions.Web) ?? [];

    var engine = app.Services.GetRequiredService<IVerticalSlicesEngine>();
    await engine.Setup(features);
}

app.MapGet("/", () => "VerticalSlices Engine is running.");

await app.RunAsync();
