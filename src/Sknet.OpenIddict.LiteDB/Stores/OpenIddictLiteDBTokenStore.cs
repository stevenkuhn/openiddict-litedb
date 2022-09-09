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
/// Provides methods allowing to manage the tokens stored in a database.
/// </summary>
/// <typeparam name="TToken">The type of the Token entity.</typeparam>
public class OpenIddictLiteDBTokenStore<TToken> : IOpenIddictTokenStore<TToken>
    where TToken : OpenIddictLiteDBToken
{
    /// <summary>
    /// Creates a new instance of the <see cref="OpenIddictLiteDBTokenStore{TToken}"/> class.
    /// </summary>
    public OpenIddictLiteDBTokenStore(
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
        var collection = database.GetCollection<TToken>(Options.CurrentValue.TokensCollectionName);

        return collection.LongCount();
    }

    /// <inheritdoc/>
    public virtual async ValueTask<long> CountAsync<TResult>(
        Func<IQueryable<TToken>, IQueryable<TResult>> query, CancellationToken cancellationToken)
    {
        if (query is null)
        {
            throw new ArgumentNullException(nameof(query));
        }

        var database = await Context.GetDatabaseAsync(cancellationToken);
        var collection = database.GetCollection<TToken>(Options.CurrentValue.TokensCollectionName);

        return query(collection.FindAll().AsQueryable()).LongCount();
    }

    /// <inheritdoc/>
    public virtual async ValueTask CreateAsync(TToken token, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        var database = await Context.GetDatabaseAsync(cancellationToken);
        var collection = database.GetCollection<TToken>(Options.CurrentValue.TokensCollectionName);

        collection.Insert(token);
    }

    /// <inheritdoc/>
    public virtual async ValueTask DeleteAsync(TToken token, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        var database = await Context.GetDatabaseAsync(cancellationToken);
        var collection = database.GetCollection<TToken>(Options.CurrentValue.TokensCollectionName);

        if (collection.DeleteMany(entity =>
            entity.Id == token.Id &&
            entity.ConcurrencyToken == token.ConcurrencyToken) == 0)
        {
            throw new ConcurrencyException("The token was concurrently updated and cannot be persisted in its current state.\r\nReload the token from the database and retry the operation.");
        }
    }

    /// <inheritdoc/>
    public virtual IAsyncEnumerable<TToken> FindAsync(string subject,
        string client, CancellationToken cancellationToken)
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

        async IAsyncEnumerable<TToken> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var database = await Context.GetDatabaseAsync(cancellationToken);
            var collection = database.GetCollection<TToken>(Options.CurrentValue.TokensCollectionName);

            var tokens = collection.Query()
                .Where(entity =>
                    entity.ApplicationId == new ObjectId(client) &&
                    entity.Subject == subject)
                .ToEnumerable().ToAsyncEnumerable();

            await foreach (var token in tokens)
            {
                yield return token;
            }
        }
    }

    /// <inheritdoc/>
    public virtual IAsyncEnumerable<TToken> FindAsync(
        string subject, string client,
        string status, CancellationToken cancellationToken)
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

        async IAsyncEnumerable<TToken> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var database = await Context.GetDatabaseAsync(cancellationToken);
            var collection = database.GetCollection<TToken>(Options.CurrentValue.TokensCollectionName);

            var tokens = collection.Query()
                .Where(entity =>
                    entity.ApplicationId == new ObjectId(client) &&
                    entity.Subject == subject &&
                    entity.Status == status)
                .ToEnumerable().ToAsyncEnumerable();

            await foreach (var token in tokens)
            {
                yield return token;
            }
        }
    }

    /// <inheritdoc/>
    public virtual IAsyncEnumerable<TToken> FindAsync(
        string subject, string client,
        string status, string type, CancellationToken cancellationToken)
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

        async IAsyncEnumerable<TToken> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var database = await Context.GetDatabaseAsync(cancellationToken);
            var collection = database.GetCollection<TToken>(Options.CurrentValue.TokensCollectionName);

            var tokens = collection.Query()
                .Where(entity =>
                    entity.ApplicationId == new ObjectId(client) &&
                    entity.Subject == subject &&
                    entity.Status == status &&
                    entity.Type == type)
                .ToEnumerable().ToAsyncEnumerable();

            await foreach (var token in tokens)
            {
                yield return token;
            }
        }
    }

    /// <inheritdoc/>
    public virtual IAsyncEnumerable<TToken> FindByApplicationIdAsync(string identifier, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            throw new ArgumentException("The identifier cannot be null or empty.", nameof(identifier));
        }

        return ExecuteAsync(cancellationToken);

        async IAsyncEnumerable<TToken> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var database = await Context.GetDatabaseAsync(cancellationToken);
            var collection = database.GetCollection<TToken>(Options.CurrentValue.TokensCollectionName);

            var tokens = collection.Query()
                .Where(entity => entity.ApplicationId == new ObjectId(identifier))
                .ToEnumerable().ToAsyncEnumerable();

            await foreach (var token in tokens)
            {
                yield return token;
            }
        }
    }

    /// <inheritdoc/>
    public virtual IAsyncEnumerable<TToken> FindByAuthorizationIdAsync(string identifier, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            throw new ArgumentException("The identifier cannot be null or empty.", nameof(identifier));
        }

        return ExecuteAsync(cancellationToken);

        async IAsyncEnumerable<TToken> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var database = await Context.GetDatabaseAsync(cancellationToken);
            var collection = database.GetCollection<TToken>(Options.CurrentValue.TokensCollectionName);

            var tokens = collection.Query()
                .Where(entity => entity.AuthorizationId == new ObjectId(identifier))
                .ToEnumerable().ToAsyncEnumerable();

            await foreach (var token in tokens)
            {
                yield return token;
            }
        }
    }

    /// <inheritdoc/>
    public virtual async ValueTask<TToken?> FindByIdAsync(string identifier, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            throw new ArgumentException("The identifier cannot be null or empty.", nameof(identifier));
        }

        var database = await Context.GetDatabaseAsync(cancellationToken);
        var collection = database.GetCollection<TToken>(Options.CurrentValue.TokensCollectionName);

        return collection.FindById(identifier);
    }

    /// <inheritdoc/>
    public virtual async ValueTask<TToken?> FindByReferenceIdAsync(string identifier, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            throw new ArgumentException("The identifier cannot be null or empty.", nameof(identifier));
        }

        var database = await Context.GetDatabaseAsync(cancellationToken);
        var collection = database.GetCollection<TToken>(Options.CurrentValue.TokensCollectionName);

        return collection.Query()
            .Where(entity => entity.ReferenceId == identifier)
            .FirstOrDefault();
    }

    /// <inheritdoc/>
    public virtual IAsyncEnumerable<TToken> FindBySubjectAsync(string subject, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(subject))
        {
            throw new ArgumentException("The subject cannot be null or empty.", nameof(subject));
        }

        return ExecuteAsync(cancellationToken);

        async IAsyncEnumerable<TToken> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var database = await Context.GetDatabaseAsync(cancellationToken);
            var collection = database.GetCollection<TToken>(Options.CurrentValue.TokensCollectionName);

            var tokens = collection.Query()
                .Where(entity => entity.Subject == subject)
                .ToEnumerable().ToAsyncEnumerable();

            await foreach (var token in tokens)
            {
                yield return token;
            }
        }
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetApplicationIdAsync(TToken token, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        if (token.ApplicationId == ObjectId.Empty)
        {
            return new(result: null);
        }

        return new(token.ApplicationId.ToString());
    }

    /// <inheritdoc/>
#pragma warning disable CS8613 // Nullability of reference types in return type doesn't match implicitly implemented member.
    public virtual async ValueTask<TResult?> GetAsync<TState, TResult>(
#pragma warning restore CS8613 // Nullability of reference types in return type doesn't match implicitly implemented member.
        Func<IQueryable<TToken>, TState, IQueryable<TResult>> query,
        TState state, CancellationToken cancellationToken)
    {
        if (query is null)
        {
            throw new ArgumentNullException(nameof(query));
        }

        var database = await Context.GetDatabaseAsync(cancellationToken);
        var collection = database.GetCollection<TToken>(Options.CurrentValue.TokensCollectionName);

        return query(collection.FindAll().AsQueryable(), state).FirstOrDefault();
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetAuthorizationIdAsync(TToken token, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        if (token.AuthorizationId == ObjectId.Empty)
        {
            return new(result: null);
        }

        return new(token.AuthorizationId.ToString());
    }

    /// <inheritdoc/>
    public virtual ValueTask<DateTimeOffset?> GetCreationDateAsync(TToken token, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        if (token.CreationDate is null)
        {
            return new(result: null);
        }

        return new(DateTime.SpecifyKind(token.CreationDate.Value, DateTimeKind.Utc));
    }

    /// <inheritdoc/>
    public virtual ValueTask<DateTimeOffset?> GetExpirationDateAsync(TToken token, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        if (token.ExpirationDate is null)
        {
            return new(result: null);
        }

        return new(DateTime.SpecifyKind(token.ExpirationDate.Value, DateTimeKind.Utc));
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetIdAsync(TToken token, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        return new(token.Id.ToString());
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetPayloadAsync(TToken token, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        return new(token.Payload);
    }

    /// <inheritdoc/>
    public virtual ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(TToken token, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        if (token.Properties is null || token.Properties.Count == 0)
        {
            return new(ImmutableDictionary<string, JsonElement>.Empty);
        }

        return new(token.Properties);
    }

    /// <inheritdoc/>
    public virtual ValueTask<DateTimeOffset?> GetRedemptionDateAsync(TToken token, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        if (token.RedemptionDate is null)
        {
            return new(result: null);
        }

        return new(DateTime.SpecifyKind(token.RedemptionDate.Value, DateTimeKind.Utc));
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetReferenceIdAsync(TToken token, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        return new(token.ReferenceId);
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetStatusAsync(TToken token, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        return new(token.Status);
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetSubjectAsync(TToken token, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        return new(token.Subject);
    }

    /// <inheritdoc/>
    public virtual ValueTask<string?> GetTypeAsync(TToken token, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        return new(token.Type);
    }

    /// <inheritdoc/>
    public virtual ValueTask<TToken> InstantiateAsync(CancellationToken cancellationToken)
    {
        try
        {
            return new(Activator.CreateInstance<TToken>());
        }

        catch (MemberAccessException exception)
        {
            return new(Task.FromException<TToken>(
                new InvalidOperationException("An error occurred while trying to create a new token instance.\r\nMake sure that the token entity is not abstract and has a public parameterless constructor or create a custom token store that overrides 'InstantiateAsync()' to use a custom factory.", exception)));
        }
    }

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<TToken> ListAsync(
        int? count, int? offset, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var database = await Context.GetDatabaseAsync(cancellationToken);
        var collection = database.GetCollection<TToken>(Options.CurrentValue.TokensCollectionName);

        var tokens = collection
            .Find(Query.All(), offset ?? 0, count ?? int.MaxValue)
            .ToAsyncEnumerable();

        await foreach (var token in tokens)
        {
            yield return token;
        }
    }

    /// <inheritdoc/>
    public virtual IAsyncEnumerable<TResult> ListAsync<TState, TResult>(
        Func<IQueryable<TToken>, TState, IQueryable<TResult>> query,
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
            var collection = database.GetCollection<TToken>(Options.CurrentValue.TokensCollectionName);

            var tokens = query(collection.FindAll().AsQueryable(), state).ToAsyncEnumerable();

            await foreach (var token in tokens)
            {
                yield return token;
            }
        }
    }

    /// <inheritdoc/>
    public virtual ValueTask PruneAsync(DateTimeOffset threshold, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();

        //var database = await Context.GetDatabaseAsync(cancellationToken);
        //var collection = database.GetCollection<TToken>(Options.CurrentValue.TokensCollectionName);

        //// Note: directly deleting the resulting set of an aggregate query is not supported by MongoDb.
        //// To work around this limitation, the token identifiers are stored in an intermediate list
        //// and delete requests are sent to remove the documents corresponding to these identifiers.

        //var identifiers =
        //    await (from token in collection.AsQueryable()
        //           join authorization in database.GetCollection<OpenIddictLiteDBAuthorization>(Options.CurrentValue.AuthorizationsCollectionName).AsQueryable()
        //                              on token.AuthorizationId equals authorization.Id into authorizations
        //           where token.CreationDate < threshold.UtcDateTime
        //           where (token.Status != Statuses.Inactive && token.Status != Statuses.Valid) ||
        //                  token.ExpirationDate < DateTime.UtcNow ||
        //                  authorizations.Any(authorization => authorization.Status != Statuses.Valid)
        //           select token.Id).ToListAsync(cancellationToken);

        //// Note: to avoid generating delete requests with very large filters, a buffer is used here and the
        //// maximum number of elements that can be removed by a single call to PruneAsync() is deliberately limited.
        //foreach (var buffer in Buffer(identifiers.Take(1_000_000), 1_000))
        //{
        //    await collection.DeleteManyAsync(token => buffer.Contains(token.Id), cancellationToken);
        //}

        //static IEnumerable<List<TSource>> Buffer<TSource>(IEnumerable<TSource> source, int count)
        //{
        //    List<TSource>? buffer = null;

        //    foreach (var element in source)
        //    {
        //        buffer ??= new List<TSource>(capacity: 1);
        //        buffer.Add(element);

        //        if (buffer.Count == count)
        //        {
        //            yield return buffer;

        //            buffer = null;
        //        }
        //    }

        //    if (buffer is not null)
        //    {
        //        yield return buffer;
        //    }
        //}
    }

    /// <inheritdoc/>
    public virtual ValueTask SetApplicationIdAsync(TToken token, string? identifier, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        if (!string.IsNullOrEmpty(identifier))
        {
            token.ApplicationId = new ObjectId(identifier);
        }
        else
        {
            token.ApplicationId = ObjectId.Empty;
        }

        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetAuthorizationIdAsync(TToken token, string? identifier, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        if (!string.IsNullOrEmpty(identifier))
        {
            token.AuthorizationId = new ObjectId(identifier);
        }
        else
        {
            token.AuthorizationId = ObjectId.Empty;
        }

        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetCreationDateAsync(TToken token, DateTimeOffset? date, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        token.CreationDate = date?.UtcDateTime;
        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetExpirationDateAsync(TToken token, DateTimeOffset? date, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        token.ExpirationDate = date?.UtcDateTime;
        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetPayloadAsync(TToken token, string? payload, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        token.Payload = payload;
        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetPropertiesAsync(TToken token,
        ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        if (properties is not { Count: > 0 })
        {
            token.Properties = null;
            return default;
        }

        token.Properties = properties;
        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetRedemptionDateAsync(TToken token, DateTimeOffset? date, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        token.RedemptionDate = date?.UtcDateTime;
        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetReferenceIdAsync(TToken token, string? identifier, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        token.ReferenceId = identifier;
        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetStatusAsync(TToken token, string? status, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        token.Status = status;
        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetSubjectAsync(TToken token, string? subject, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        token.Subject = subject;
        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask SetTypeAsync(TToken token, string? type, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        token.Type = type;
        return default;
    }

    /// <inheritdoc/>
    public virtual async ValueTask UpdateAsync(TToken token, CancellationToken cancellationToken)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        // Generate a new concurrency token and attach it
        // to the token before persisting the changes.
        var timestamp = token.ConcurrencyToken;
        token.ConcurrencyToken = Guid.NewGuid().ToString();

        var database = await Context.GetDatabaseAsync(cancellationToken);
        var collection = database.GetCollection<TToken>(Options.CurrentValue.TokensCollectionName);

        if (collection.Count(entity => entity.Id == token.Id && entity.ConcurrencyToken == timestamp) == 0)
        {
            throw new ConcurrencyException("The token was concurrently updated and cannot be persisted in its current state.\r\nReload the token from the database and retry the operation.");
        }

        collection.Update(token);
    }
}
