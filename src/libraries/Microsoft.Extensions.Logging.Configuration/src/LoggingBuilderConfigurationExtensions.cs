// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.Logging.Configuration
{
    /// <summary>
    /// Extension methods for setting up logging services in an <see cref="ILoggingBuilder" />.
    /// </summary>
    public static class LoggingBuilderConfigurationExtensions
    {
        /// <summary>
        /// Adds services required to consume <see cref="ILoggerProviderConfigurationFactory"/> or <see cref="ILoggerProviderConfiguration{T}"/>
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to register services on.</param>
        [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode",
            Justification = "ServiceDescriptor has RequiresUnreferencedCode because Service and Implementation types could be generic with mis-matching trimming annotations. " +
            "All types used here have matching trimming annotations.")]
        public static void AddConfiguration(this ILoggingBuilder builder)
        {
            builder.Services.TryAddSingleton<ILoggerProviderConfigurationFactory, LoggerProviderConfigurationFactory>();
            builder.Services.TryAddSingleton(typeof(ILoggerProviderConfiguration<>), typeof(LoggerProviderConfiguration<>));
        }
    }
}
