// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices;

/// <summary>
/// Defines a service that validates a set of modules and their vertical slices
/// before code generation begins.
/// </summary>
public interface ISliceValidator
{
    /// <summary>
    /// Validates all slices reachable from the given modules.
    /// </summary>
    /// <param name="modules">The modules to validate.</param>
    /// <exception cref="SliceValidationFailed">Thrown when one or more validation errors are found.</exception>
    void Validate(IEnumerable<Module> modules);
}
