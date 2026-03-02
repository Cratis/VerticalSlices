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
    /// Uses convention-based registration for services following the <c>IFoo</c> → <c>Foo</c> naming convention,
    /// and registers defaults for code output (<see cref="NoOpCodeOutput"/>) and Chronicle integration (<see cref="NoOpChronicleRegistration"/>).
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>A <see cref="VerticalSlicesBuilder"/> to further configure the engine.</returns>
    public static VerticalSlicesBuilder AddVerticalSlices(this IServiceCollection services)
    {
        // Register default no-op implementations that can be overridden by the consumer.
        services.AddSingleton<ICodeOutput, NoOpCodeOutput>();
        services.AddSingleton<IChronicleRegistration, NoOpChronicleRegistration>();

        // Register all ISliceTypeCodeGenerator implementations explicitly since they share one interface.
        services.AddSingleton<ISliceTypeCodeGenerator, StateChangeCodeGenerator>();
        services.AddSingleton<ISliceTypeCodeGenerator, StateViewCodeGenerator>();
        services.AddSingleton<ISliceTypeCodeGenerator, AutomationCodeGenerator>();
        services.AddSingleton<ISliceTypeCodeGenerator, TranslatorCodeGenerator>();

        // Convention-based registration handles:
        // IVerticalSlicesEngine → VerticalSlicesEngine
        // IVerticalSliceCodeGenerator → VerticalSliceCodeGenerator
        // ICodeOutputResolver → CodeOutputResolver
        // IChronicleRegistrationResolver → ChronicleRegistrationResolver
        // IEventModelAdvisor → EventModelAdvisor
        // ISliceValidator → SliceValidator
        services.AddBindingsByConvention();

        return new VerticalSlicesBuilder(services);
    }
}
