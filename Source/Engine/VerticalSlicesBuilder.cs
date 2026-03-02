// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.Chronicle;
using Cratis.VerticalSlices.CodeGeneration.Output;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cratis.VerticalSlices;

/// <summary>
/// A fluent builder for configuring the Vertical Slices engine.
/// Obtained by calling <see cref="ServiceCollectionExtensions.AddVerticalSlices"/>.
/// </summary>
/// <param name="services">The <see cref="IServiceCollection"/> to configure.</param>
public class VerticalSlicesBuilder(IServiceCollection services)
{
    /// <summary>
    /// Configures the engine to write generated code files to a local directory on disk,
    /// replacing the default no-op output.
    /// </summary>
    /// <param name="outputRoot">The root directory to write generated files into.</param>
    /// <returns>This builder for continuation.</returns>
    public VerticalSlicesBuilder WithLocalFileSystemOutput(string outputRoot)
    {
        services.AddSingleton<ICodeOutput>(provider =>
            new LocalFileSystemOutput(outputRoot, provider.GetRequiredService<ILogger<LocalFileSystemOutput>>()));

        return this;
    }

    /// <summary>
    /// Configures the engine to register artifacts with a Chronicle instance over its HTTP REST API,
    /// replacing the default no-op Chronicle integration.
    /// </summary>
    /// <param name="options">The connection options for the Chronicle HTTP API.</param>
    /// <returns>This builder for continuation.</returns>
    public VerticalSlicesBuilder WithChronicleHttp(ChronicleHttpOptions options)
    {
        services.AddSingleton<IChronicleRegistration>(provider =>
        {
            var httpClient = new HttpClient { BaseAddress = new Uri(options.BaseUrl) };
            var logger = provider.GetRequiredService<ILogger<ChronicleHttpRegistration>>();
            return new ChronicleHttpRegistration(httpClient, options, logger);
        });

        return this;
    }

    /// <summary>
    /// Configures the engine to register artifacts with a Chronicle instance over its HTTP REST API,
    /// replacing the default no-op Chronicle integration.
    /// </summary>
    /// <param name="baseUrl">The base URL of the Chronicle HTTP API (e.g. <c>http://localhost:8080</c>).</param>
    /// <param name="eventStore">The name of the event store to register artifacts in.</param>
    /// <param name="namespace">The namespace within the event store. Defaults to <c>Default</c>.</param>
    /// <returns>This builder for continuation.</returns>
    public VerticalSlicesBuilder WithChronicleHttp(string baseUrl, string eventStore, string @namespace = "Default") =>
        WithChronicleHttp(new ChronicleHttpOptions(baseUrl, eventStore, @namespace));
}
