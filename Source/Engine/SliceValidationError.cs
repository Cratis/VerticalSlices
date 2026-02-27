// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents a single validation error found in a vertical slice.
/// </summary>
/// <param name="SliceName">The name of the slice that failed validation.</param>
/// <param name="SliceType">The architectural type of the slice.</param>
/// <param name="Message">A human-readable description of the violation.</param>
public record SliceValidationError(string SliceName, VerticalSliceType SliceType, string Message);
