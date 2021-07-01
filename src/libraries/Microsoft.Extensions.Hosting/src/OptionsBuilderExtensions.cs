// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for adding configuration related options services to the DI container via <see cref="OptionsBuilder{TOptions}"/>.
    /// </summary>
    public static class OptionsBuilderExtensions
    {
        /// <summary>
        /// Enforces options validation check on start rather than in runtime.
        /// </summary>
        /// <typeparam name="TOptions">The type of options.</typeparam>
        /// <param name="optionsBuilder">The <see cref="OptionsBuilder{TOptions}"/> to configure options instance.</param>
        /// <returns>The <see cref="OptionsBuilder{TOptions}"/> so that additional calls can be chained.</returns>
        public static OptionsBuilder<TOptions> ValidateOnStart<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TOptions>(this OptionsBuilder<TOptions> optionsBuilder)
            where TOptions : class
        {
            if (optionsBuilder == null)
            {
                throw new ArgumentNullException(nameof(optionsBuilder));
            }

            optionsBuilder.Services.AddHostedService<ValidationHostedService>();

            ConfigureOptionsClosure<TOptions> closure = new ConfigureOptionsClosure<TOptions>(optionsBuilder);
            optionsBuilder.Services.AddOptions<ValidatorOptions>()
                .Configure(new Action<ValidatorOptions, IOptionsMonitor<TOptions>>(closure.ConfigureOptions));

            return optionsBuilder;
        }

        private readonly struct ConfigureOptionsClosure<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TOptions>
            where TOptions : class
        {
            private readonly OptionsBuilder<TOptions> _optionsBuilder;

            public ConfigureOptionsClosure(OptionsBuilder<TOptions> optionsBuilder)
            {
                _optionsBuilder = optionsBuilder;
            }

            public void ConfigureOptions(ValidatorOptions vo, IOptionsMonitor<TOptions> options)
            {
                // This adds an action that resolves the options value to force evaluation
                // We don't care about the result as duplicates are not important
                GetOptionsClosure<TOptions> closure = new GetOptionsClosure<TOptions>(options, _optionsBuilder);
                vo.Validators[typeof(TOptions)] = new Action(closure.GetOptions);
            }
        }

        private readonly struct GetOptionsClosure<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TOptions>
            where TOptions : class
        {
            private readonly IOptionsMonitor<TOptions> _options;
            private readonly OptionsBuilder<TOptions> _optionsBuilder;

            public GetOptionsClosure(IOptionsMonitor<TOptions> options, OptionsBuilder<TOptions> optionsBuilder)
            {
                _options = options;
                _optionsBuilder = optionsBuilder;
            }

            public void GetOptions()
            {
                _options.Get(_optionsBuilder.Name);
            }
        }
    }
}
