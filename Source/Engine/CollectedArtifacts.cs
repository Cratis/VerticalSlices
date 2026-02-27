// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.CodeGeneration;
using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices;

/// <summary>
/// Holds the artifacts that were collected when traversing the module/feature hierarchy.
/// Returned by the internal collection pass in <see cref="VerticalSlicesEngine"/> so that
/// no mutable lists need to be passed by reference between recursive calls.
/// </summary>
/// <param name="Files">The generated source files.</param>
/// <param name="EventDescriptors">The event type descriptors gathered for Chronicle registration.</param>
/// <param name="ReadModelDescriptors">The read model descriptors gathered for Chronicle registration.</param>
public record CollectedArtifacts(
    IReadOnlyList<GeneratedFile> Files,
    IReadOnlyList<EventTypeDescriptor> EventDescriptors,
    IReadOnlyList<ReadModelDescriptor> ReadModelDescriptors);
