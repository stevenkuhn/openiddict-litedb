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
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Sknet.OpenIddict.LiteDB;

/// <summary>
/// Provides methods allowing to manage the authorizations stored in a database.
/// </summary>
/// <typeparam name="TAuthorization">The type of the Authorization entity.</typeparam>
public class OpenIddictLiteDBAuthorizationStore<TAuthorization> : IOpenIddictAuthorizationStore<TAuthorization>
    where TAuthorization : OpenIddictLiteDBAuthorization
{
    /// <summary>
    /// Creates a new instance of the <see cref="OpenIddictLiteDBAuthorizationStore{TAuthorization}"/> class.
    /// </summary>
    public OpenIddictLiteDBAuthorizationStore(
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
        var collection = database.GetCollection<TAuthorization>(Options.CurrentValue.AuthorizationsCollectionName);

        return collection.LongCount();
    }

    /// <inheritdoc/>
    public virtual async ValueTask<long> CountAsync<TResult>(
        Func<IQueryable<TAuthorization>, IQueryable<TResult>> query, CancellationToken cancellationToken)
    {
        if (query is null)
        {
            throw new ArgumentNullException(nameof(query));
        }

        var database = await Context.GetDatabaseAsync(cancellationToken);
        var collection = database.GetCollection<TAuthorization>(Options.CurrentValue.AuthorizationsCollectionName);

        return query(collection.FindAll().AsQueryable()).LongCount();
    }

    /// <inheritdoc/>
    public virtual async ValueTask CreateAsync(TAuthorization authorization, CancellationToken cancellationToken)
    {
        if (authorization is null)
        {
            throw new ArgumentNullException(nameof(authorization));
        }

        var database = await Context.GetDatabaseAsync(cancellationToken);
        var collection = database.GetCollection<TAuthorization>(Options.CurrentValue.AuthorizationsCollectionName);

        collection.Insert(authorization);
    }

    /// <inheritdoc/>
    public virtual async ValueTask DeleteAsync(TAuthorization authorization, CancellationToken cancellationToken)
    {
        if (authorization is null)
        {
            throw new ArgumentNullException(nameof(authorization));
        }

        var database = await Context.GetDatabaseAsync(cancellationToken);
        var collection = database.GetCollection<TAuthorization>(Options.CurrentValue.AuthorizationsCollectionName);

        if (collection.DeleteMany(entity =>
            entity.Id == authorization.Id &&
            entity.ConcurrencyToken == authorization.ConcurrencyToken) == 0)
        {
            throw new OpenIddictExceptions.ConcurrencyException("The authorization was concurrently updated and cannot be persisted in its current state.\r\nReload the authorization from the database and retry the operation.");
        }

        // Delete the tokens associated with the authorization.
        database.GetCollection<OpenIddictLiteDBToken>(Options.CurrentValue.TokensCollectionName)
            .DeleteMany(token => token.AuthorizationId == authorization.Id);
    }

    /// <inheritdoc/>
    public virtual IAsyncEnumerable<TAuthorization> FindAsync(
        string subject, string client, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(subject))
        {
            throw new ArgumentException("The subject cannot be null or empty.", nameof(subject));
        }

        if (string.IsNullOrEmpty(client))
        {
            throw new ArgumentException("The client identifier cannot be null or empty.", nameof(client));
        }

        return ExecuteAsync(cancellationToken);

        async IAsyncEnumerable<TAuthorization> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var database = await Context.GetDatabaseAsync(cancellationToken);
            var collection = database.GetCollection<TAuthorization>(Options.CurrentValue.AuthorizationsCollectionName);

            var authorizations = collection.Query()
                .Where(entity =>
                    entity.Subject == subject &&
                    entity.ApplicationId == new ObjectId(client))
                .ToEnumerable().ToAsyncEnumerable();

            await foreach (var authorization in authorizations)
            {
                yield return authorization;
            }
        }
    }

    /// <inheritdoc/>
    public virtual IAsyncEnumerable<TAuthorization> FindAsync(
        string subject, string client, string status, 
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(subject))
        {
            throw new ArgumentException("The subject cannot be null or empty.", nameof(subject));
        }

        if (string.IsNullOrEmpty(client))
        {
            throw new ArgumentException("The client identifier cannot be null or empty.", nameof(client));
        }

        if (string.IsNullOrEmpty(status))
        {
            throw new ArgumentException("The status cannot be null or empty.", nameof(status));
        }

        return ExecuteAsync(cancellationToken);

        async IAsyncEnumerable<TAuthorization> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var database = await Context.GetDatabaseAsync(cancellationToken);
            var collection = database.GetCollection<TAuthorization>(Options.CurrentValue.AuthorizationsCollectionName);

            var authorizations = collection.Query()
                .Where(entity =>
                    entity.Subject == subject &&
                    entity.ApplicationId == new ObjectId(client) &&
                    entity.Status == status)
                .ToEnumerable().ToAsyncEnumerable();

            await foreach (var authorization in authorizations)
            {
                yield return authorization;
            }
        }
    }

    /// <inheritdoc/>
    public virtual IAsyncEnumerable<TAuthorization> FindAsync(
        string subject, string client, string status, string type,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(subject))
        {
            throw new ArgumentException("The subject cannot be null or empty.", nameof(subject));
        }

        if (string.IsNullOrEmpty(client))
        {
            throw new ArgumentException("The client identifier cannot be null or empty.", nameof(client));
        }

        if (string.IsNullOrEmpty(status))
        {
            throw new ArgumentException("The status cannot be null or empty.", nameof(status));
        }

        if (string.IsNullOrEmpty(type))
        {
            throw new ArgumentException("The type cannot be null or empty.", nameof(type));
        }

        return ExecuteAsync(cancellationToken);

        async IAsyncEnumerable<TAuthorization> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var database = await Context.GetDatabaseAsync(cancellationToken);
            var collection = database.GetCollection<TAuthorization>(Options.CurrentValue.AuthorizationsCollectionName);

            var authorizations = collection.Query()
                .Where(entity =>
                    entity.Subject == subject &&
                    entity.ApplicationId == new ObjectId(client) &&
                    entity.Status == status &&
                    entity.Type == type)
                .ToEnumerable().ToAsyncEnumerable();

            await foreach (var authorization in authorizations)
            {
                yield return authorization;
            }
        }
    }

    /// <inheritdoc/>
    public virtual IAsyncEnumerable<TAuthorization> FindAsync(
        string subject, string client, string status, string type,
        ImmutableArray<string> scopes, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(subject))
        {
            throw new ArgumentException("The subject cannot be null or empty.", nameof(subject));
        }

        if (string.IsNullOrEmpty(client))
        {
            throw new ArgumentException("The client identifier cannot be null or empty.", nameof(client));
        }

        if (string.IsNullOrEmpty(status))
        {
            throw new ArgumentException("The status cannot be null or empty.", nameof(status));
        }
        
        if (string.IsNullOrEmpty(type))
        {
            throw new ArgumentException("The type cannot be null or empty.", nameof(type));
        }

        return ExecuteAsync(cancellationToken);

        async IAsyncEnumerable<TAuthorization> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var database = await Context.GetDatabaseAsync(cancellationToken);
            var collection = database.GetCollection<TAuthorization>(Options.CurrentValue.AuthorizationsCollectionName);
          
            var authorizations = collection.Query()
                .Where(entity =>
                    entity.Subject == subject &&
                    entity.ApplicationId == new ObjectId(client) &&
                    entity.Status == status &&
                    entity.Type == type)
                .ToEnumerable()
                .Where(entity => scopes.All(scope => entity.Scopes!.Contains(scope)))
                .ToAsyncEnumerable();

            await foreach (var authorization in authorizations)
            {
                yield return authorization;
            }
        }
    }

    /// <inheritdoc/>
    public virtual IAsyncEnumerable<TAuthorization> FindByApplicationIdAsync(
        string identifier, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            throw new ArgumentException("The identifier cannot be null or empty.", nameof(identifier));
        }

        return ExecuteAsync(cancellationToken);

        async IAsyncEnumerable<TAuthorization> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var database = await Context.GetDatabaseAsync(cancellationToken);
            var collection = database.GetCollection<TAuthorization>(Options.CurrentValue.AuthorizationsCollectionName);

            var authorizations = collection.Query()
                .Where(entity => entity.ApplicationId == new ObjectId(identifier))
                .ToEnumerable().ToAsyncEnumerable();

            await foreach (var authorization in authorizations)
            {
                yield return authorization;
            }
        }
    }

    /// <inheritdoc/>
    public virtual async ValueTask<TAuthorization?> FindByIdAsync(string identifier, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            throw new ArgumentException("The identifier cannot be null or empty.", nameof(identifier));
        }

        var database = await Context.GetDatabaseAsync(cancellationToken);
        var collection = database.GetCollection<TAuthorization>(Options.CurrentValue.AuthorizationsCollectionName);

        return collection.FindById(new ObjectId(identifier));
    }

    /// <inheritdoc/>
    public virtual IAsyncEnumerable<TAuthorization> FindBySubjectAsync(
        string subject, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(subject))
        {
            throw new ArgumentException("The subject cannot be null or empty.", nameof(subject));
        }

        return ExecuteAsync(cancellationToken);

        async IAsyncEnumerable<TAuthorization> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var database = await Context.GetDatabaseAsync(cancellationToken);
            var collection = database.GetCollection<TAuthorization>(Options.CurrentValue.AuthorizationsCollectionName);

            var authorizations = collection.Query()
                .Where(entity => entity.Subject == subject)
                .ToEnumerable().ToAsyncEnumerable();

            await foreach (var authorization in authorizations)
            {
                yield return authorization;
            }
        }
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetApplicationIdAsync(TAuthorization authorization, CancellationToken cancellationToken)
    {
        if (authorization is null)
        {
            throw new ArgumentNullException(nameof(authorization));
        }

        if (authorization.ApplicationId == ObjectId.Empty)
        {
            return new(result: null);
        }

        return new(authorization.ApplicationId.ToString());
    }

    /// <inheritdoc/>
#pragma warning disable CS8613 // Nullability of reference types in return type doesn't match implicitly implemented member.
    public virtual async ValueTask<TResult?> GetAsync<TState, TResult>(
#pragma warning restore CS8613 // Nullability of reference types in return type doesn't match implicitly implemented member.
        Func<IQueryable<TAuthorization>, TState, IQueryable<TResult>> query,
        TState state, CancellationToken cancellationToken)
    {
        if (query is null)
        {
            throw new ArgumentNullException(nameof(query));
        }

        var database = await Context.GetDatabaseAsync(cancellationToken);
        var collection = database.GetCollection<TAuthorization>(Options.CurrentValue.AuthorizationsCollectionName);

        return query(collection.FindAll().AsQueryable(), state).FirstOrDefault();
    }

    /// <inheritdoc/>
    public virtual ValueTask<DateTimeOffset?> GetCreationDateAsync(TAuthorization authorization, CancellationToken cancellationToken)
    {
        if (authorization is null)
        {
            throw new ArgumentNullException(nameof(authorization));
        }

        if (authorization.CreationDate is null)
        {
            return new(result: null);
        }

        return new(authorization.CreationDate);
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetIdAsync(TAuthorization authorization, CancellationToken cancellationToken)
    {
        if (authorization is null)
        {
            throw new ArgumentNullException(nameof(authorization));
        }

        return new(authorization.Id.ToString());
    }

    /// <inheritdoc/>
    public virtual ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(TAuthorization authorization, CancellationToken cancellationToken)
    {
        if (authorization is null)
        {
            throw new ArgumentNullException(nameof(authorization));
        }

        if (authorization.Properties is null || authorization.Properties.Count == 0)
        {
            return new(ImmutableDictionary<string, JsonElement>.Empty);
        }

        return new(authorization.Properties);
    }

    /// <inheritdoc/>
    public virtual ValueTask<ImmutableArray<string>> GetScopesAsync(TAuthorization authorization, CancellationToken cancellationToken)
    {
        if (authorization is null)
        {
            throw new ArgumentNullException(nameof(authorization));
        }

        if (authorization.Scopes is not { Length: > 0 })
        {
            return new(ImmutableArray<string>.Empty);
        }

        return new(authorization.Scopes.Value);
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetStatusAsync(TAuthorization authorization, CancellationToken cancellationToken)
    {
        if (authorization is null)
        {
            throw new ArgumentNullException(nameof(authorization));
        }

        return new(authorization.Status);
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetSubjectAsync(TAuthorization authorization, CancellationToken cancellationToken)
    {
        if (authorization is null)
        {
            throw new ArgumentNullException(nameof(authorization));
        }

        return new(authorization.Subject);
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetTypeAsync(TAuthorization authorization, CancellationToken cancellationToken)
    {
        if (authorization is null)
        {
            throw new ArgumentNullException(nameof(authorization));
        }

        return new(authorization.Type);
    }

    /// <inheritdoc/>
    public virtual ValueTask<TAuthorization> InstantiateAsync(CancellationToken cancellationToken)
    {
        try
        {
            return new(Activator.CreateInstance<TAuthorization>());
        }

        catch (MemberAccessException exception)
        {
            return new(Task.FromException<TAuthorization>(
                new InvalidOperationException("An error occurred while trying to create a new authorization instance.\r\nMake sure that the authorization entity is not abstract and has a public parameterless constructor or create a custom authorization store that overrides 'InstantiateAsync()' to use a custom factory.", exception)));
        }
    }

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<TAuthorization> ListAsync(
        int? count, int? offset, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var database = await Context.GetDatabaseAsync(cancellationToken);
        var collection = database.GetCollection<TAuthorization>(Options.CurrentValue.AuthorizationsCollectionName);

        var authorizations = collection
            .Find(Query.All(), offset ?? 0, count ?? int.MaxValue)
            .ToAsyncEnumerable();

        await foreach (var authorization in authorizations)
        {
            yield return authorization;
        }
    }

    /// <inheritdoc/>
    public virtual IAsyncEnumerable<TResult> ListAsync<TState, TResult>(
        Func<IQueryable<TAuthorization>, TState, IQueryable<TResult>> query,
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
            var collection = database.GetCollection<TAuthorization>(Options.CurrentValue.AuthorizationsCollectionName);

            var authorizations = query(collection.FindAll().AsQueryable(), state).ToAsyncEnumerable();

            await foreach (var authorization in authorizations)
            {
                yield return authorization;
            }
        }
    }
    
    /// <inheritdoc/>
    public async virtual ValueTask PruneAsync(DateTimeOffset threshold, CancellationToken cancellationToken)
    {
        var database = await Context.GetDatabaseAsync(cancellationToken);
        var authorizationCollection = database.GetCollection<TAuthorization>(Options.CurrentValue.AuthorizationsCollectionName);

        var query = from auth in authorizationCollection.Query()
                    // only prune authorizations created before threshold
                    where auth.CreationDate < threshold.UtcDateTime
                    // prune tokens that are not valid
                    where auth.Status != Statuses.Valid
                    select auth.Id;
        var authorizations = query.ToList();

        // prune adhoc authorizations that have no tokens
        query = from auth in authorizationCollection.Query()
                where auth.CreationDate < threshold.UtcDateTime
                where auth.Status == Statuses.Valid
                where auth.Type == AuthorizationTypes.AdHoc
                select auth.Id;
        var adhocAuthorizations = new HashSet<ObjectId>(query.ToEnumerable());

        var tokenCollection = database.GetCollection(Options.CurrentValue.TokensCollectionName);
        var adhocAuthorizationsWithTokens = new HashSet<ObjectId>(tokenCollection.Query()
            .GroupBy("authorization_id")
            .Where(x => adhocAuthorizations.Contains(x["authorization_id"]))
            .Select("{authorization_id:@key}")
            .ToEnumerable()
            .Select(x => x["authorization_id"].AsObjectId));

        adhocAuthorizations.SymmetricExceptWith(adhocAuthorizationsWithTokens);
        authorizations.AddRange(adhocAuthorizations);

        database.BeginTrans();
        foreach (var authorization in authorizations)
        {
            authorizationCollection.Delete(authorization);
        }
        database.Commit();
    }

    /// <inheritdoc/>
    public virtual ValueTask SetApplicationIdAsync(TAuthorization authorization,
        string? identifier, CancellationToken cancellationToken)
    {
        if (authorization is null)
        {
            throw new ArgumentNullException(nameof(authorization));
        }

        if (!string.IsNullOrEmpty(identifier))
        {
            authorization.ApplicationId = new ObjectId(identifier);
        }
        else
        {
            authorization.ApplicationId = ObjectId.Empty;
        }

        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetCreationDateAsync(TAuthorization authorization,
        DateTimeOffset? date, CancellationToken cancellationToken)
    {
        if (authorization is null)
        {
            throw new ArgumentNullException(nameof(authorization));
        }

        authorization.CreationDate = date?.UtcDateTime;
        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetPropertiesAsync(TAuthorization authorization,
        ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken)
    {
        if (authorization is null)
        {
            throw new ArgumentNullException(nameof(authorization));
        }

        if (properties is not { Count: > 0 })
        {
            authorization.Properties = null;
            return default;
        }

        authorization.Properties = properties;
        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetScopesAsync(TAuthorization authorization,
        ImmutableArray<string> scopes, CancellationToken cancellationToken)
    {
        if (authorization is null)
        {
            throw new ArgumentNullException(nameof(authorization));
        }

        if (scopes.IsDefaultOrEmpty)
        {
            authorization.Scopes = null;
            return default;
        }

        authorization.Scopes = scopes;
        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetStatusAsync(TAuthorization authorization, string? status, CancellationToken cancellationToken)
    {
        if (authorization is null)
        {
            throw new ArgumentNullException(nameof(authorization));
        }

        authorization.Status = status;
        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetSubjectAsync(TAuthorization authorization, string? subject, CancellationToken cancellationToken)
    {
        if (authorization is null)
        {
            throw new ArgumentNullException(nameof(authorization));
        }

        authorization.Subject = subject;
        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetTypeAsync(TAuthorization authorization, string? type, CancellationToken cancellationToken)
    {
        if (authorization is null)
        {
            throw new ArgumentNullException(nameof(authorization));
        }

        authorization.Type = type;
        return default;
    }

    /// <inheritdoc/>
    public virtual async ValueTask UpdateAsync(TAuthorization authorization, CancellationToken cancellationToken)
    {
        if (authorization is null)
        {
            throw new ArgumentNullException(nameof(authorization));
        }

        // Generate a new concurrency token and attach it
        // to the authorization before persisting the changes.
        var timestamp = authorization.ConcurrencyToken;
        authorization.ConcurrencyToken = Guid.NewGuid().ToString();

        var database = await Context.GetDatabaseAsync(cancellationToken);
        var collection = database.GetCollection<TAuthorization>(Options.CurrentValue.AuthorizationsCollectionName);

        if (collection.Count(entity => entity.Id == authorization.Id && entity.ConcurrencyToken == timestamp) == 0)
        {
            throw new ConcurrencyException("The authorization was concurrently updated and cannot be persisted in its current state.\r\nReload the authorization from the database and retry the operation.");
        }

        collection.Update(authorization);
    }
}

