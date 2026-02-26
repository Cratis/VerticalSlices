// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.Chronicle;

/// <summary>
/// Configuration options for connecting to a Chronicle instance over HTTP.
/// </summary>
/// <param name="BaseUrl">The base URL of the Chronicle HTTP API (e.g. http://localhost:8080).</param>
/// <param name="EventStore">The name of the event store to register artifacts in.</param>
/// <param name="Namespace">The namespace within the event store. Defaults to "Default".</param>
public record ChronicleHttpOptions(string BaseUrl, string EventStore, string Namespace = "Default");
