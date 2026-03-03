// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices;

/// <summary>
/// A test-friendly <see cref="IServiceProvider"/> that creates instances using <see cref="Activator.CreateInstance(Type)"/>.
/// </summary>
sealed class ActivatorServiceProvider : IServiceProvider
{
    /// <inheritdoc/>
    public object? GetService(Type serviceType) => Activator.CreateInstance(serviceType);
}
