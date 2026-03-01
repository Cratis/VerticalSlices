// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.Chronicle;

/// <summary>
/// Defines a resolver for determining which <see cref="IChronicleRegistration"/> to use
/// based on configuration and consumer requirements.
/// </summary>
public interface IChronicleRegistrationResolver
{
    /// <summary>
    /// Resolves the Chronicle registration strategy to use for registering artifacts.
    /// When no real registration is configured, returns a <see cref="NoOpChronicleRegistration"/>.
    /// </summary>
    /// <returns>The resolved <see cref="IChronicleRegistration"/>.</returns>
    IChronicleRegistration Resolve();
}
