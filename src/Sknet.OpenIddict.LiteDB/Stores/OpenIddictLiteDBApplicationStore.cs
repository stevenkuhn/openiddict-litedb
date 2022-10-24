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
namespace Sknet.OpenIddict.LiteDB;

/// <summary>
/// Provides methods allowing to manage the applications stored in a database.
/// </summary>
/// <typeparam name="TApplication">The type of the Application entity.</typeparam>
public class OpenIddictLiteDBApplicationStore<TApplication> : IOpenIddictApplicationStore<TApplication>
    where TApplication : OpenIddictLiteDBApplication
{
    /// <summary>
    /// Creates a new instance of the <see cref="OpenIddictLiteDBApplicationStore{TApplication}"/> class.
    /// </summary>
    public OpenIddictLiteDBApplicationStore(
        IOpenIddictLiteDBContext context,
        IOptionsMonitor<OpenIddictLiteDBOptions> options)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Gets the database context associated with the current store.
    /// </summary>
    protected IOpenIddictLiteDBContext Context { get; }

    /// <summary>
    /// Gets the options associated with the current store.
    /// </summary>
    protected IOptionsMonitor<OpenIddictLiteDBOptions> Options { get; }

    /// <inheritdoc/>
    public virtual async ValueTask<long> CountAsync(CancellationToken cancellationToken)
    {
        var database = await Context.GetDatabaseAsync(cancellationToken);
        var collection = database.GetCollection<TApplication>(Options.CurrentValue.ApplicationsCollectionName);

        return collection.LongCount();
    }

    /// <inheritdoc/>
    public virtual async ValueTask<long> CountAsync<TResult>(
        Func<IQueryable<TApplication>, IQueryable<TResult>> query, CancellationToken cancellationToken)
    {
        if (query is null)
        {
            throw new ArgumentNullException(nameof(query));
        }

        var database = await Context.GetDatabaseAsync(cancellationToken);
        var collection = database.GetCollection<TApplication>(Options.CurrentValue.ApplicationsCollectionName);

        return query(collection.FindAll().AsQueryable()).LongCount();
    }

    /// <inheritdoc/>
    public virtual async ValueTask CreateAsync(TApplication application, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        var database = await Context.GetDatabaseAsync(cancellationToken);
        var collection = database.GetCollection<TApplication>(Options.CurrentValue.ApplicationsCollectionName);

        collection.Insert(application);
    }

    /// <inheritdoc/>
    public virtual async ValueTask DeleteAsync(TApplication application, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        var database = await Context.GetDatabaseAsync(cancellationToken);
        var collection = database.GetCollection<TApplication>(Options.CurrentValue.ApplicationsCollectionName);

        if (collection.DeleteMany(entity =>
            entity.Id == application.Id &&
            entity.ConcurrencyToken == application.ConcurrencyToken) == 0)
        {
            throw new OpenIddictExceptions.ConcurrencyException("The application was concurrently updated and cannot be persisted in its current state.\r\nReload the application from the database and retry the operation.");
        }

        // Delete the authorizations associated with the application.
        database.GetCollection<OpenIddictLiteDBAuthorization>(Options.CurrentValue.AuthorizationsCollectionName)
            .DeleteMany(authorization => authorization.ApplicationId == application.Id);

        // Delete the tokens associated with the application.
        database.GetCollection<OpenIddictLiteDBToken>(Options.CurrentValue.TokensCollectionName)
            .DeleteMany(token => token.ApplicationId == application.Id);
    }

    /// <inheritdoc/>
    public virtual async ValueTask<TApplication?> FindByClientIdAsync(string identifier, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            throw new ArgumentException("The identifier cannot be null or empty.", nameof(identifier));
        }

        var database = await Context.GetDatabaseAsync(cancellationToken);
        var collection = database.GetCollection<TApplication>(Options.CurrentValue.ApplicationsCollectionName);

        return collection.Find(application => application.ClientId == identifier).FirstOrDefault();
    }

    /// <inheritdoc/>
    public virtual async ValueTask<TApplication?> FindByIdAsync(string identifier, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            throw new ArgumentException("The identifier cannot be null or empty.", nameof(identifier));
        }

        var database = await Context.GetDatabaseAsync(cancellationToken);
        var collection = database.GetCollection<TApplication>(Options.CurrentValue.ApplicationsCollectionName);

        return collection.FindById(new ObjectId(identifier));
    }

    /// <inheritdoc/>
    public virtual IAsyncEnumerable<TApplication> FindByPostLogoutRedirectUriAsync(
        string address, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(address))
        {
            throw new ArgumentException("The address cannot be null or empty.", nameof(address));
        }

        return ExecuteAsync(cancellationToken);

        async IAsyncEnumerable<TApplication> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var database = await Context.GetDatabaseAsync(cancellationToken);
            var collection = database.GetCollection<TApplication>(Options.CurrentValue.ApplicationsCollectionName);

            var applications = collection.Query()
                .Where(entity => entity.PostLogoutRedirectUris != null && entity.PostLogoutRedirectUris.Contains(address))
                .ToEnumerable().ToAsyncEnumerable();

            await foreach (var application in applications)
            {
                yield return application;
            }
        }
    }

    /// <inheritdoc/>
    public virtual IAsyncEnumerable<TApplication> FindByRedirectUriAsync(
        string address, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(address))
        {
            throw new ArgumentException("The address cannot be null or empty.", nameof(address));
        }

        return ExecuteAsync(cancellationToken);

        async IAsyncEnumerable<TApplication> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var database = await Context.GetDatabaseAsync(cancellationToken);
            var collection = database.GetCollection<TApplication>(Options.CurrentValue.ApplicationsCollectionName);

            var applications = collection.Query()
                .Where(entity => entity.RedirectUris != null && entity.RedirectUris.Contains(address))
                .ToEnumerable().ToAsyncEnumerable();

            await foreach (var application in applications)
            {
                yield return application;
            }
        }
    }

    /// <inheritdoc/>
#pragma warning disable CS8613 // Nullability of reference types in return type doesn't match implicitly implemented member.
    public virtual async ValueTask<TResult?> GetAsync<TState, TResult>(
#pragma warning restore CS8613 // Nullability of reference types in return type doesn't match implicitly implemented member.
        Func<IQueryable<TApplication>, TState, IQueryable<TResult>> query,
        TState state, CancellationToken cancellationToken)
    {
        if (query is null)
        {
            throw new ArgumentNullException(nameof(query));
        }

        var database = await Context.GetDatabaseAsync(cancellationToken);
        var collection = database.GetCollection<TApplication>(Options.CurrentValue.ApplicationsCollectionName);

        return query(collection.FindAll().AsQueryable(), state).FirstOrDefault();
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetClientIdAsync(TApplication application, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        return new(application.ClientId);
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetClientSecretAsync(TApplication application, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        return new(application.ClientSecret);
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetClientTypeAsync(TApplication application, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        return new(application.Type);
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetConsentTypeAsync(TApplication application, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        return new(application.ConsentType);
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetDisplayNameAsync(TApplication application, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        return new(application.DisplayName);
    }

    /// <inheritdoc/>
    public virtual ValueTask<ImmutableDictionary<CultureInfo, string>> GetDisplayNamesAsync(TApplication application, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        if (application.DisplayNames is not { Count: > 0 })
        {
            return new(ImmutableDictionary<CultureInfo, string>.Empty);
        }

        return new(application.DisplayNames);
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetIdAsync(TApplication application, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        return new(application.Id.ToString());
    }

    /// <inheritdoc/>
    public virtual ValueTask<ImmutableArray<string>> GetPermissionsAsync(
        TApplication application, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        if (application.Permissions is not { Length: > 0 })
        {
            return new(ImmutableArray<string>.Empty);
        }

        return new(application.Permissions.Value);
    }

    /// <inheritdoc/>
    public virtual ValueTask<ImmutableArray<string>> GetPostLogoutRedirectUrisAsync(
        TApplication application, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        if (application.PostLogoutRedirectUris is not { Length: > 0 })
        {
            return new(ImmutableArray<string>.Empty);
        }

        return new(application.PostLogoutRedirectUris.Value);
    }

    /// <inheritdoc/>
    public virtual ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(TApplication application, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        if (application.Properties is null || application.Properties.Count == 0)
        {
            return new(ImmutableDictionary<string, JsonElement>.Empty);
        }

        return new(application.Properties);
    }

    /// <inheritdoc/>
    public virtual ValueTask<ImmutableArray<string>> GetRedirectUrisAsync(
        TApplication application, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        if (application.RedirectUris is not { Length: > 0 })
        {
            return new(ImmutableArray<string>.Empty);
        }

        return new(application.RedirectUris.Value);
    }

    /// <inheritdoc/>
    public virtual ValueTask<ImmutableArray<string>> GetRequirementsAsync(TApplication application, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        if (application.Requirements is not { Length: > 0 })
        {
            return new(ImmutableArray<string>.Empty);
        }

        return new(application.Requirements.Value);
    }

    /// <inheritdoc/>
    public virtual ValueTask<TApplication> InstantiateAsync(CancellationToken cancellationToken)
    {
        try
        {
            return new(Activator.CreateInstance<TApplication>());
        }

        catch (MemberAccessException exception)
        {
            return new(Task.FromException<TApplication>(
                new InvalidOperationException("An error occurred while trying to create a new application instance.\r\nMake sure that the application entity is not abstract and has a public parameterless constructor or create a custom application store that overrides 'InstantiateAsync()' to use a custom factory.", exception)));
        }
    }

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<TApplication> ListAsync(
        int? count, int? offset, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var database = await Context.GetDatabaseAsync(cancellationToken);
        var collection = database.GetCollection<TApplication>(Options.CurrentValue.ApplicationsCollectionName);

        var applications = collection
            .Find(Query.All(), offset ?? 0, count ?? int.MaxValue)
            .ToAsyncEnumerable();

        await foreach (var application in applications)
        {
            yield return application;
        }
    }

    /// <inheritdoc/>
    public virtual IAsyncEnumerable<TResult> ListAsync<TState, TResult>(
        Func<IQueryable<TApplication>, TState, IQueryable<TResult>> query,
        TState state, CancellationToken cancellationToken)
    {
        if (query is null)
        {
            throw new ArgumentNullException(nameof(query));
        }

        return ExecuteAsync(cancellationToken);

        async IAsyncEnumerable<TResult> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var database = await Context.GetDatabaseAsync(cancellationToken);
            var collection = database.GetCollection<TApplication>(Options.CurrentValue.ApplicationsCollectionName);

            var applications = query(collection.FindAll().AsQueryable(), state).ToAsyncEnumerable();

            await foreach (var application in applications)
            {
                yield return application;
            }
        }
    }

    /// <inheritdoc/>
    public virtual ValueTask SetClientIdAsync(TApplication application,
        string? identifier, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        application.ClientId = identifier;

        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetClientSecretAsync(TApplication application,
        string? secret, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        application.ClientSecret = secret;

        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetClientTypeAsync(TApplication application,
        string? type, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        application.Type = type;

        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetConsentTypeAsync(TApplication application,
        string? type, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        application.ConsentType = type;

        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetDisplayNameAsync(TApplication application,
        string? name, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        application.DisplayName = name;

        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetDisplayNamesAsync(TApplication application,
        ImmutableDictionary<CultureInfo, string> names, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        if (names is not { Count: > 0 })
        {
            application.DisplayNames = null;
            return default;
        }

        application.DisplayNames = names;
        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetPermissionsAsync(TApplication application, ImmutableArray<string> permissions, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        if (permissions.IsDefaultOrEmpty)
        {
            application.Permissions = null;
            return default;
        }

        application.Permissions = permissions;
        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetPostLogoutRedirectUrisAsync(TApplication application,
        ImmutableArray<string> addresses, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        if (addresses.IsDefaultOrEmpty)
        {
            application.PostLogoutRedirectUris = null;
            return default;
        }

        application.PostLogoutRedirectUris = addresses;
        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetPropertiesAsync(TApplication application,
        ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        if (properties is not { Count: > 0 })
        {
            application.Properties = null;
            return default;
        }

        application.Properties = properties;
        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetRedirectUrisAsync(TApplication application,
        ImmutableArray<string> addresses, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        if (addresses.IsDefaultOrEmpty)
        {
            application.RedirectUris = null;
            return default;
        }

        application.RedirectUris = addresses;
        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetRequirementsAsync(TApplication application,
        ImmutableArray<string> requirements, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        if (requirements.IsDefaultOrEmpty)
        {
            application.Requirements = null;
            return default;
        }

        application.Requirements = requirements;
        return default;
    }

    /// <inheritdoc/>
    public virtual async ValueTask UpdateAsync(TApplication application, CancellationToken cancellationToken)
    {
        if (application is null)
        {
            throw new ArgumentNullException(nameof(application));
        }

        // Generate a new concurrency token and attach it
        // to the application before persisting the changes.
        var timestamp = application.ConcurrencyToken;
        application.ConcurrencyToken = Guid.NewGuid().ToString();

        var database = await Context.GetDatabaseAsync(cancellationToken);
        var collection = database.GetCollection<TApplication>(Options.CurrentValue.ApplicationsCollectionName);

        if (collection.Count(entity => entity.Id == application.Id && entity.ConcurrencyToken == timestamp) == 0)
        {
            throw new ConcurrencyException("The application was concurrently updated and cannot be persisted in its current state.\r\nReload the application from the database and retry the operation.");
        }

        collection.Update(application);
    }
}
