// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.Chronicle;

/// <summary>
/// An <see cref="IChronicleRegistrationResolver"/> that resolves to a single configured
/// <see cref="IChronicleRegistration"/> instance. When no registration is configured,
/// resolves to a <see cref="NoOpChronicleRegistration"/>.
/// </summary>
/// <param name="registration">The Chronicle registration to resolve to. When not provided, defaults to <see cref="NoOpChronicleRegistration"/>.</param>
public class ChronicleRegistrationResolver(IChronicleRegistration? registration = null) : IChronicleRegistrationResolver
{
    /// <inheritdoc/>
    public IChronicleRegistration Resolve() => registration ?? new NoOpChronicleRegistration();
}
