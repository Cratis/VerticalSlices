// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices;

/// <summary>
/// Placeholder for future Given-When-Then specification support.
/// In Event Modeling, each vertical slice can have zero or more specifications
/// that describe preconditions (given events), an action (when command), and
/// expected outcomes (then events or read model state).
/// </summary>
/// <param name="Name">The name of the specification.</param>
/// <param name="Description">An optional description of what scenario this specification covers.</param>
public record GivenWhenThen(string Name, string? Description = null);
