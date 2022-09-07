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
/// Exposes the necessary methods required to configure the OpenIddict LiteDB services.
/// </summary>
public class OpenIddictLiteDBBuilder
{
    /// <summary>
    /// Initializes a new instance of <see cref="OpenIddictLiteDBBuilder"/>.
    /// </summary>
    /// <param name="services">The services collection.</param>
    public OpenIddictLiteDBBuilder(IServiceCollection services)
        => Services = services ?? throw new ArgumentNullException(nameof(services));

    /// <summary>
    /// Gets the services collection.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public IServiceCollection Services { get; }

    /// <summary>
    /// Amends the default OpenIddict LiteDB configuration.
    /// </summary>
    /// <param name="configuration">The delegate used to configure the OpenIddict options.</param>
    /// <remarks>This extension can be safely called multiple times.</remarks>
    /// <returns>The <see cref="OpenIddictLiteDBBuilder"/>.</returns>
    public OpenIddictLiteDBBuilder Configure(Action<OpenIddictLiteDBOptions> configuration)
    {
        if (configuration is null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        Services.Configure(configuration);

        return this;
    }

    /// <summary>
    /// Configures OpenIddict to use the specified entity as the default application entity.
    /// </summary>
    /// <returns>The <see cref="OpenIddictLiteDBBuilder"/>.</returns>
    public OpenIddictLiteDBBuilder ReplaceDefaultApplicationEntity<TApplication>()
        where TApplication : OpenIddictLiteDBApplication
    {
        Services.Configure<OpenIddictCoreOptions>(options => options.DefaultApplicationType = typeof(TApplication));

        return this;
    }

    /// <summary>
    /// Configures OpenIddict to use the specified entity as the default authorization entity.
    /// </summary>
    /// <returns>The <see cref="OpenIddictLiteDBBuilder"/>.</returns>
    public OpenIddictLiteDBBuilder ReplaceDefaultAuthorizationEntity<TAuthorization>()
        where TAuthorization : OpenIddictLiteDBAuthorization
    {
        Services.Configure<OpenIddictCoreOptions>(options => options.DefaultAuthorizationType = typeof(TAuthorization));

        return this;
    }

    /// <summary>
    /// Configures OpenIddict to use the specified entity as the default scope entity.
    /// </summary>
    /// <returns>The <see cref="OpenIddictLiteDBBuilder"/>.</returns>
    public OpenIddictLiteDBBuilder ReplaceDefaultScopeEntity<TScope>()
        where TScope : OpenIddictLiteDBScope
    {
        Services.Configure<OpenIddictCoreOptions>(options => options.DefaultScopeType = typeof(TScope));

        return this;
    }

    /// <summary>
    /// Configures OpenIddict to use the specified entity as the default token entity.
    /// </summary>
    /// <returns>The <see cref="OpenIddictLiteDBBuilder"/>.</returns>
    public OpenIddictLiteDBBuilder ReplaceDefaultTokenEntity<TToken>()
        where TToken : OpenIddictLiteDBToken
    {
        Services.Configure<OpenIddictCoreOptions>(options => options.DefaultTokenType = typeof(TToken));

        return this;
    }

    /// <summary>
    /// Replaces the default applications collection name (by default, openiddict.applications).
    /// </summary>
    /// <param name="name">The collection name</param>
    /// <returns>The <see cref="OpenIddictLiteDBBuilder"/>.</returns>
    public OpenIddictLiteDBBuilder SetApplicationsCollectionName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("The collection name cannot be null or empty.", nameof(name));
        }

        return Configure(options => options.ApplicationsCollectionName = name);
    }

    /// <summary>
    /// Replaces the default authorizations collection name (by default, openiddict.authorizations).
    /// </summary>
    /// <param name="name">The collection name</param>
    /// <returns>The <see cref="OpenIddictLiteDBBuilder"/>.</returns>
    public OpenIddictLiteDBBuilder SetAuthorizationsCollectionName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("The collection name cannot be null or empty.", nameof(name));
        }

        return Configure(options => options.AuthorizationsCollectionName = name);
    }

    /// <summary>
    /// Replaces the default scopes collection name (by default, openiddict.scopes).
    /// </summary>
    /// <param name="name">The collection name</param>
    /// <returns>The <see cref="OpenIddictLiteDBBuilder"/>.</returns>
    public OpenIddictLiteDBBuilder SetScopesCollectionName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("The collection name cannot be null or empty.", nameof(name));
        }

        return Configure(options => options.ScopesCollectionName = name);
    }

    /// <summary>
    /// Replaces the default tokens collection name (by default, openiddict.tokens).
    /// </summary>
    /// <param name="name">The collection name</param>
    /// <returns>The <see cref="OpenIddictLiteDBBuilder"/>.</returns>
    public OpenIddictLiteDBBuilder SetTokensCollectionName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("The collection name cannot be null or empty.", nameof(name));
        }

        return Configure(options => options.TokensCollectionName = name);
    }

    /// <summary>
    /// Configures the LiteDB stores to use the specified database
    /// instead of retrieving it from the dependency injection container.
    /// </summary>
    /// <param name="database">The <see cref="ILiteDatabase"/>.</param>
    /// <returns>The <see cref="OpenIddictLiteDBBuilder"/>.</returns>
    public OpenIddictLiteDBBuilder UseDatabase(ILiteDatabase database)
    {
        if (database is null)
        {
            throw new ArgumentNullException(nameof(database));
        }

        return Configure(options => options.Database = database);
    }

    /// <inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object? obj) => base.Equals(obj);

    /// <inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => base.GetHashCode();

    /// <inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override string? ToString() => base.ToString();
}