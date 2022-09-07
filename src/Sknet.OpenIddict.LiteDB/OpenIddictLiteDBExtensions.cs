/*
 * Copyright (c) 2022 Steven Kuhn and contributors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace Microsoft.Extensions.DependencyInjection;


/// <summary>
/// Exposes extensions allowing to register the OpenIddict LiteDB services.
/// </summary>
public static class OpenIddictLiteDBExtensions
{
    /// <summary>
    /// Registers the LiteDB stores services in the DI container and
    /// configures OpenIddict to use the LiteDB entities by default.
    /// </summary>
    /// <param name="builder">The services builder used by OpenIddict to register new services.</param>
    /// <remarks>This extension can be safely called multiple times.</remarks>
    /// <returns>The <see cref="OpenIddictLiteDBBuilder"/>.</returns>
    public static OpenIddictLiteDBBuilder UseLiteDB(this OpenIddictCoreBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        // Note: Mongo uses simple binary comparison checks by default so the additional
        // query filtering applied by the default OpenIddict managers can be safely disabled.
        // builder.DisableAdditionalFiltering();

        builder.SetDefaultApplicationEntity<OpenIddictLiteDBApplication>()
               .SetDefaultAuthorizationEntity<OpenIddictLiteDBAuthorization>()
               .SetDefaultScopeEntity<OpenIddictLiteDBScope>()
               .SetDefaultTokenEntity<OpenIddictLiteDBToken>();

        // Note: the LiteDB stores/resolvers don't depend on scoped/transient services and thus
        // can be safely registered as singleton services and shared/reused across requests.
        builder.ReplaceApplicationStoreResolver<OpenIddictLiteDBApplicationStoreResolver>(ServiceLifetime.Singleton)
               .ReplaceAuthorizationStoreResolver<OpenIddictLiteDBAuthorizationStoreResolver>(ServiceLifetime.Singleton)
               .ReplaceScopeStoreResolver<OpenIddictLiteDBScopeStoreResolver>(ServiceLifetime.Singleton)
               .ReplaceTokenStoreResolver<OpenIddictLiteDBTokenStoreResolver>(ServiceLifetime.Singleton);

        builder.Services.TryAddSingleton(typeof(OpenIddictLiteDBApplicationStore<>));
        builder.Services.TryAddSingleton(typeof(OpenIddictLiteDBAuthorizationStore<>));
        builder.Services.TryAddSingleton(typeof(OpenIddictLiteDBScopeStore<>));
        builder.Services.TryAddSingleton(typeof(OpenIddictLiteDBTokenStore<>));

        builder.Services.TryAddSingleton<IOpenIddictLiteDBContext, OpenIddictLiteDBContext>();

        return new OpenIddictLiteDBBuilder(builder.Services);
    }

    /// <summary>
    /// Registers the LiteDB stores services in the DI container and
    /// configures OpenIddict to use the LiteDB entities by default.
    /// </summary>
    /// <param name="builder">The services builder used by OpenIddict to register new services.</param>
    /// <param name="configuration">The configuration delegate used to configure the LiteDB services.</param>
    /// <remarks>This extension can be safely called multiple times.</remarks>
    /// <returns>The <see cref="OpenIddictCoreBuilder"/>.</returns>
    public static OpenIddictCoreBuilder UseLiteDB(
        this OpenIddictCoreBuilder builder, Action<OpenIddictLiteDBBuilder> configuration)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (configuration is null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        configuration(builder.UseLiteDB());

        return builder;
    }
}