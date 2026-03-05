// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net.Http.Json;
using System.Text.Json;
using Cratis.VerticalSlices.CodeGeneration.Descriptors;
using Microsoft.Extensions.Logging;

namespace Cratis.VerticalSlices.Chronicle;

/// <summary>
/// An <see cref="IChronicleRegistration"/> that registers artifacts with a Chronicle instance
/// over its HTTP REST API.
/// </summary>
/// <param name="httpClient">The HTTP client configured to talk to the Chronicle API.</param>
/// <param name="options">The connection options.</param>
/// <param name="logger">The logger.</param>
public partial class ChronicleHttpRegistration(
    HttpClient httpClient,
    ChronicleHttpOptions options,
    ILogger<ChronicleHttpRegistration> logger) : IChronicleRegistration
{
    static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    /// <inheritdoc/>
    public async Task RegisterEventTypes(IEnumerable<EventTypeDescriptor> eventTypes, CancellationToken ct = default)
    {
        foreach (var eventType in eventTypes)
        {
            ct.ThrowIfCancellationRequested();

            var url = $"/api/event-store/{options.EventStore}/types/create";
            var body = new { name = eventType.Name };

            var response = await httpClient.PostAsJsonAsync(url, body, _jsonOptions, ct);

            if (response.IsSuccessStatusCode)
            {
                LogRegisteredEventType(eventType.Name);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync(ct);
                LogEventTypeRegistrationFailed(eventType.Name, (int)response.StatusCode, content);
            }
        }
    }

    /// <inheritdoc/>
    public async Task RegisterProjections(IEnumerable<ReadModelDescriptor> readModels, CancellationToken ct = default)
    {
        foreach (var readModel in readModels)
        {
            ct.ThrowIfCancellationRequested();

            var pdl = PdlGenerator.Generate(readModel);
            const string url = "/api/cratis/chronicle/api/projections/save-projection";
            var body = new
            {
                eventStore = options.EventStore,
                @namespace = options.Namespace,
                declaration = pdl,
                draftReadModel = (object?)null
            };

            var response = await httpClient.PostAsJsonAsync(url, body, _jsonOptions, ct);

            if (response.IsSuccessStatusCode)
            {
                LogRegisteredProjection(readModel.Name);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync(ct);
                LogProjectionRegistrationFailed(readModel.Name, (int)response.StatusCode, content);
            }
        }
    }

    /// <inheritdoc/>
    public async Task RegisterReadModelTypes(IEnumerable<ReadModelDescriptor> readModels, CancellationToken ct = default)
    {
        foreach (var readModel in readModels)
        {
            ct.ThrowIfCancellationRequested();

            var schema = JsonSchemaGenerator.Generate(
                readModel.Properties.Select(p => new Property(p.Name, p.Type)));

            var url = $"/api/event-store/{options.EventStore}/read-model-types/create";
            var body = new
            {
                identifier = readModel.Name,
                displayName = readModel.Name,
                containerName = readModel.Name,
                schema
            };

            var response = await httpClient.PostAsJsonAsync(url, body, _jsonOptions, ct);

            if (response.IsSuccessStatusCode)
            {
                LogRegisteredReadModelType(readModel.Name);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync(ct);
                LogReadModelTypeRegistrationFailed(readModel.Name, (int)response.StatusCode, content);
            }
        }
    }

    [LoggerMessage(LogLevel.Information, "Registered event type '{EventTypeName}' with Chronicle")]
    partial void LogRegisteredEventType(string eventTypeName);

    [LoggerMessage(LogLevel.Warning, "Failed to register event type '{EventTypeName}' with Chronicle (HTTP {StatusCode}): {Response}")]
    partial void LogEventTypeRegistrationFailed(string eventTypeName, int statusCode, string response);

    [LoggerMessage(LogLevel.Information, "Registered projection for '{ReadModelName}' with Chronicle")]
    partial void LogRegisteredProjection(string readModelName);

    [LoggerMessage(LogLevel.Warning, "Failed to register projection for '{ReadModelName}' with Chronicle (HTTP {StatusCode}): {Response}")]
    partial void LogProjectionRegistrationFailed(string readModelName, int statusCode, string response);

    [LoggerMessage(LogLevel.Information, "Registered read model type '{ReadModelName}' with Chronicle")]
    partial void LogRegisteredReadModelType(string readModelName);

    [LoggerMessage(LogLevel.Warning, "Failed to register read model type '{ReadModelName}' with Chronicle (HTTP {StatusCode}): {Response}")]
    partial void LogReadModelTypeRegistrationFailed(string readModelName, int statusCode, string response);
}
