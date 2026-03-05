// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.DependencyInjection;
using Cratis.VerticalSlices;
using Cratis.VerticalSlices.Chronicle;
using Cratis.VerticalSlices.CodeGeneration.Output;
using Cratis.VerticalSlices.CodeGeneration.SliceTypes;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> for registering the Vertical Slices engine and related services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the Vertical Slices engine and all required services with the IoC container.
    /// Uses convention-based registration for services following the <c>IFoo</c> → <c>Foo</c> naming convention.
    /// Configure code output and Chronicle targets by passing <see cref="CodeOutputOptions"/> and
    /// <see cref="ChronicleOptions"/> directly to <see cref="IVerticalSlicesEngine.Process"/>.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> for fluent chaining.</returns>
    public static IServiceCollection AddVerticalSlices(this IServiceCollection services)
    {
        // Set up type discovery — registers ITypes, IInstancesOf<> and IImplementationsOf<>
        // so that convention-registered singletons (e.g. all IEventModelRule implementations)
        // are discoverable via IInstancesOf<T> without any manual registration by the consumer.
        services.AddTypeDiscovery();

        // Register all ISliceTypeCodeGenerator implementations explicitly since they share one interface.
        services.AddSingleton<ISliceTypeCodeGenerator, StateChangeCodeGenerator>();
        services.AddSingleton<ISliceTypeCodeGenerator, StateViewCodeGenerator>();
        services.AddSingleton<ISliceTypeCodeGenerator, AutomationCodeGenerator>();
        services.AddSingleton<ISliceTypeCodeGenerator, TranslatorCodeGenerator>();

        // Convention-based registration handles:
        // IVerticalSlicesEngine → VerticalSlicesEngine
        // IVerticalSliceCodeGenerator → VerticalSliceCodeGenerator
        // IEventModelAdvisor → EventModelAdvisor
        // Also self-registers all [Singleton]-adorned IEventModelRule implementations
        // so they are resolvable by IInstancesOf<IEventModelRule>.
        services.AddBindingsByConvention();

        return services;
    }
}
