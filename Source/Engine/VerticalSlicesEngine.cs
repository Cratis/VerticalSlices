// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents an engine for setting up vertical slices with Chronicle.
/// </summary>
/// <param name="logger">The logger.</param>
public partial class VerticalSlicesEngine(ILogger<VerticalSlicesEngine> logger) : IVerticalSlicesEngine
{
    /// <inheritdoc/>
    public async Task Setup(IEnumerable<Feature> features)
    {
        foreach (var feature in features)
        {
            await SetupFeature(feature);
        }
    }

    async Task SetupFeature(Feature feature)
    {
        LogSettingUpFeature(feature.Name);

        foreach (var slice in feature.VerticalSlices)
        {
            await SetupVerticalSlice(slice);
        }

        foreach (var subFeature in feature.Features)
        {
            await SetupFeature(subFeature);
        }
    }

    async Task SetupVerticalSlice(VerticalSlice slice)
    {
        LogSettingUpVerticalSlice(slice.Name);

        foreach (var eventType in slice.Events)
        {
            await SetupEventType(eventType);
        }

        foreach (var readModel in slice.ReadModels)
        {
            await SetupProjection(slice, readModel);
        }
    }

    Task SetupEventType(EventType eventType)
    {
        LogRegisteringEventType(eventType.Name);
        return Task.CompletedTask;
    }

    Task SetupProjection(VerticalSlice slice, ReadModel readModel)
    {
        LogSettingUpProjection(readModel.Name, slice.Name);
        return Task.CompletedTask;
    }

    [LoggerMessage(LogLevel.Information, "Setting up feature {FeatureName}")]
    partial void LogSettingUpFeature(string featureName);

    [LoggerMessage(LogLevel.Information, "Setting up vertical slice {SliceName}")]
    partial void LogSettingUpVerticalSlice(string sliceName);

    [LoggerMessage(LogLevel.Information, "Registering event type {EventTypeName}")]
    partial void LogRegisteringEventType(string eventTypeName);

    [LoggerMessage(LogLevel.Information, "Setting up projection for read model {ReadModelName} in slice {SliceName}")]
    partial void LogSettingUpProjection(string readModelName, string sliceName);
}
