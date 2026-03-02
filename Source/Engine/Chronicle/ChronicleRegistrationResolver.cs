// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.DependencyInjection;

namespace Cratis.VerticalSlices.Chronicle;

/// <summary>
/// An <see cref="IChronicleRegistrationResolver"/> that resolves to a single configured
/// <see cref="IChronicleRegistration"/> instance.
/// </summary>
/// <param name="registration">The Chronicle registration to resolve to.</param>
[Singleton]
public class ChronicleRegistrationResolver(IChronicleRegistration registration) : IChronicleRegistrationResolver
{
    /// <inheritdoc/>
    public IChronicleRegistration Resolve() => registration;
}
