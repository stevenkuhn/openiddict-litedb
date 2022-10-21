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
namespace Sknet.OpenIddict.LiteDB.Tests.Stores;

public class OpenIddictLiteDBAuthorizationStoreTests
{
    [Fact]
    public async Task CountAsync_WithZeroItems_ReturnsZero()
    {
        // Arrange
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder().Build();

        // Act
        var result = await store.CountAsync(default);

        // Assert
        Assert.Equal(0, result);

    }

    [Fact]
    public async Task CountAsync_WithThreeItems_ReturnsThree()
    {
        // Arrange
        var authorizations = new OpenIddictLiteDBAuthorizationFaker().Generate(3);
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder()
            .WithAuthorizations(authorizations)
            .Build();

        // Act
        var result = await store.CountAsync(default);

        // Assert
        Assert.Equal(3, result);
    }

    [Fact]
    public async Task CountAync_WithNullQuery_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBApplicationStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            "query",
            () => store.CountAsync<object>(null!, default).AsTask());
    }

    [Fact]
    public async Task CountAsync_WithQuery_ReturnsAuthorizationCount()
    {
        // Arrange
        var authorizations = new OpenIddictLiteDBAuthorizationFaker().Generate(3);
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder()
            .WithAuthorizations(authorizations)
            .Build();

        // Act
        var result = await store.CountAsync(
            query => query.Where(a => a.ApplicationId == authorizations[1].ApplicationId),
            default);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task CreateAsync_WithNullAuthorization_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            "authorization",
            () => store.CreateAsync(null!, default).AsTask());
    }

    [Fact]
    public async Task CreateAsync_AddsAuthorizationToDatabase()
    {
        // Arrange
        var database = new OpenIddictLiteDatabase(":memory:");
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder()
            .WithDatabase(database)
            .Build();
        var authorization = new OpenIddictLiteDBAuthorizationFaker().Generate();

        // Act
        await store.CreateAsync(authorization, default);

        // Assert
        var result = database.Authorizations().FindById(authorization.Id);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task DeleteAsync_WithNullAuthorizaton_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBApplicationStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            "application",
            () => store.DeleteAsync(null!, default).AsTask());
    }

    [Fact]
    public async Task DeleteAsync_RemovesAuthorizatonFromDatabase()
    {
        // Arrange
        var authorizations = new OpenIddictLiteDBAuthorizationFaker().Generate(2);
        var database = new OpenIddictLiteDatabase(":memory:");
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder()
            .WithAuthorizations(authorizations)
            .WithDatabase(database)
            .Build();

        // Act
        await store.DeleteAsync(authorizations[0], default);

        // Assert
        var result = database.Authorizations().FindById(authorizations[0].Id);
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WithConcurrencyTokenChange_ThrowsException()
    {
        // Arrange
        var authorization = new OpenIddictLiteDBAuthorizationFaker().Generate();
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder()
            .WithAuthorizations(authorization)
            .Build();

        authorization.ConcurrencyToken = Guid.NewGuid().ToString();

        // Act/Assert
        await Assert.ThrowsAsync<OpenIddictExceptions.ConcurrencyException>(
            () => store.DeleteAsync(authorization, default).AsTask());
    }

    [Fact]
    public async Task DeleteAsync_RemovesTokensFromDatabase()
    {
        // Arrange
        var authorizations = new OpenIddictLiteDBAuthorizationFaker().Generate(2);
        var tokens = new OpenIddictLiteDBTokenFaker()
            .RuleFor(x => x.ApplicationId, f => authorizations[f.IndexFaker % 2].ApplicationId)
            .RuleFor(x => x.AuthorizationId, f => authorizations[f.IndexFaker % 2].Id)
            .Generate(2);
        var database = new OpenIddictLiteDatabase(":memory:");
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder()
            .WithAuthorizations(authorizations)
            .WithTokens(tokens)
            .WithDatabase(database)
            .Build();

        // Assert
        var authorizationId = authorizations[0].Id;
        var tokensResult = database.Tokens().Find(x => x.AuthorizationId == authorizationId);
        Assert.NotEmpty(tokensResult);

        // Act
        await store.DeleteAsync(authorizations[0], default);

        // Assert
        tokensResult = database.Tokens().Find(x => x.AuthorizationId == authorizationId);
        Assert.Empty(tokensResult);
    }

    [Fact]
    public async Task FindAsyc_WithNullSubjectAndClient_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "subject",
            () => store.FindAsync(null!, "client", default).ToListAsync().AsTask());
    }

    [Fact]
    public async Task FindAsyc_WithSubjectAndNullClient_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "client",
            () => store.FindAsync("subject", null!, default).ToListAsync().AsTask());
    }

    [Fact]
    public async Task FindAsync_WithSubjectAndClient_ShouldReturnAuthorization()
    {
        // Arrange
        var authorizations = new OpenIddictLiteDBAuthorizationFaker().Generate(3);
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder()
            .WithAuthorizations(authorizations)
            .Build();

        // Assert
        Assert.NotNull(authorizations[1].Subject);
        Assert.NotEqual(ObjectId.Empty, authorizations[1].ApplicationId);

        // Act
        var result = await store.FindAsync(authorizations[1].Subject!, authorizations[1].ApplicationId.ToString(), default).ToListAsync();

        // Assert
        Assert.Multiple(
            () => Assert.Single(result),
            () => Assert.Equal(authorizations[1].Id, result[0].Id));
    }

    [Fact]
    public async Task FindAsyc_WithNullSubjectAndClientAndStatus_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "subject",
            () => store.FindAsync(null!, "client", "status", default).ToListAsync().AsTask());
    }

    [Fact]
    public async Task FindAsyc_WithSubjectAndNullClientAndStatus_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "client",
            () => store.FindAsync("subject", null!, "status", default).ToListAsync().AsTask());
    }

    [Fact]
    public async Task FindAsyc_WithSubjectAndClientAndNullStatus_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "status",
            () => store.FindAsync("subject", "client", null!, default).ToListAsync().AsTask());
    }

    [Fact]
    public async Task FindAsync_WithSubjectAndClientAndStatus_ShouldReturnAuthorization()
    {
        // Arrange
        var authorizations = new OpenIddictLiteDBAuthorizationFaker().Generate(3);
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder()
            .WithAuthorizations(authorizations)
            .Build();

        // Assert
        Assert.NotNull(authorizations[1].Subject);
        Assert.NotEqual(ObjectId.Empty, authorizations[1].ApplicationId);
        Assert.NotNull(authorizations[1].Status);

        // Act
        var result = await store.FindAsync(
            authorizations[1].Subject!, 
            authorizations[1].ApplicationId.ToString(),
            authorizations[1].Status!, default).ToListAsync();

        // Assert
        Assert.Multiple(
            () => Assert.Single(result),
            () => Assert.Equal(authorizations[1].Id, result[0].Id));
    }

    [Fact]
    public async Task FindAsyc_WithNullSubjectAndClientAndStatusAndType_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "subject",
            () => store.FindAsync(null!, "client", "status", "type", default).ToListAsync().AsTask());
    }

    [Fact]
    public async Task FindAsyc_WithSubjectAndNullClientAndStatusAndType_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "client",
            () => store.FindAsync("subject", null!, "status", "type", default).ToListAsync().AsTask());
    }

    [Fact]
    public async Task FindAsyc_WithSubjectAndClientAndNullStatusAndType_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "status",
            () => store.FindAsync("subject", "client", null!, "type", default).ToListAsync().AsTask());
    }

    [Fact]
    public async Task FindAsyc_WithSubjectAndClientAndStatusAndNullType_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "type",
            () => store.FindAsync("subject", "client", "status", null!, default).ToListAsync().AsTask());
    }

    [Fact]
    public async Task FindAsync_WithSubjectAndClientAndStatusAndType_ShouldReturnAuthorization()
    {
        // Arrange
        var authorizations = new OpenIddictLiteDBAuthorizationFaker().Generate(3);
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder()
            .WithAuthorizations(authorizations)
            .Build();

        // Assert
        Assert.NotNull(authorizations[1].Subject);
        Assert.NotEqual(ObjectId.Empty, authorizations[1].ApplicationId);
        Assert.NotNull(authorizations[1].Status);
        Assert.NotNull(authorizations[1].Type);

        // Act
        var result = await store.FindAsync(
            authorizations[1].Subject!,
            authorizations[1].ApplicationId.ToString(),
            authorizations[1].Status!,
            authorizations[1].Type!, default).ToListAsync();

        // Assert
        Assert.Multiple(
            () => Assert.Single(result),
            () => Assert.Equal(authorizations[1].Id, result[0].Id));
    }

    [Fact]
    public async Task FindAsyc_WithNullSubjectAndClientAndStatusAndTypeAndEmptyScopes_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "subject",
            () => store.FindAsync(null!, "client", "status", "type", ImmutableArray<string>.Empty, default).ToListAsync().AsTask());
    }

    [Fact]
    public async Task FindAsyc_WithSubjectAndNullClientAndStatusAndTypeAndEmptyScopes_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "client",
            () => store.FindAsync("subject", null!, "status", "type", ImmutableArray<string>.Empty, default).ToListAsync().AsTask());
    }

    [Fact]
    public async Task FindAsyc_WithSubjectAndClientAndNullStatusAndTypeAndEmptyScopes_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "status",
            () => store.FindAsync("subject", "client", null!, "type", ImmutableArray<string>.Empty, default).ToListAsync().AsTask());
    }

    [Fact]
    public async Task FindAsyc_WithSubjectAndClientAndStatusAndNullTypeAndEmptyScopes_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "type",
            () => store.FindAsync("subject", "client", "status", null!, ImmutableArray<string>.Empty, default).ToListAsync().AsTask());
    }

    [Fact]
    public async Task FindAsyc_WithSubjectAndClientAndStatusAndTypeAndEmptyScopes_ShouldReturnAuthorization()
    {
        // Arrange
        var authorizations = new OpenIddictLiteDBAuthorizationFaker().Generate(3);
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder()
            .WithAuthorizations(authorizations)
            .Build();

        // Assert
        Assert.NotNull(authorizations[1].Subject);
        Assert.NotEqual(ObjectId.Empty, authorizations[1].ApplicationId);
        Assert.NotNull(authorizations[1].Status);
        Assert.NotNull(authorizations[1].Type);
        Assert.NotEmpty(authorizations[1].Scopes!);

        // Act
        var result = await store.FindAsync(
            authorizations[1].Subject!,
            authorizations[1].ApplicationId.ToString(),
            authorizations[1].Status!,
            authorizations[1].Type!, 
            ImmutableArray<string>.Empty, default).ToListAsync();

        // Assert
        Assert.Multiple(
            () => Assert.Single(result),
            () => Assert.Equal(authorizations[1].Id, result[0].Id));
    }

    [Fact]
    public async Task FindAsyc_WithSubjectAndClientAndStatusAndTypeAndScope_ShouldReturnAuthorization()
    {
        // Arrange
        var authorizations = new OpenIddictLiteDBAuthorizationFaker().Generate(3);
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder()
            .WithAuthorizations(authorizations)
            .Build();

        // Assert
        Assert.NotNull(authorizations[1].Subject);
        Assert.NotEqual(ObjectId.Empty, authorizations[1].ApplicationId);
        Assert.NotNull(authorizations[1].Status);
        Assert.NotNull(authorizations[1].Type);
        Assert.NotEmpty(authorizations[1].Scopes!);

        // Act
        var result = await store.FindAsync(
            authorizations[1].Subject!,
            authorizations[1].ApplicationId.ToString(),
            authorizations[1].Status!,
            authorizations[1].Type!,
            ImmutableArray.Create(authorizations[1].Scopes!.Value[0]), default).ToListAsync();

        // Assert
        Assert.Multiple(
            () => Assert.Single(result),
            () => Assert.Equal(authorizations[1].Id, result[0].Id));
    }

    [Fact]
    public async Task FindAsyc_WithSubjectAndClientAndStatusAndTypeAndInvalidScope_ShouldNotReturnAuthorization()
    {
        // Arrange
        var authorizations = new OpenIddictLiteDBAuthorizationFaker().Generate(3);
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder()
            .WithAuthorizations(authorizations)
            .Build();

        // Assert
        Assert.NotNull(authorizations[1].Subject);
        Assert.NotEqual(ObjectId.Empty, authorizations[1].ApplicationId);
        Assert.NotNull(authorizations[1].Status);
        Assert.NotNull(authorizations[1].Type);
        Assert.NotEmpty(authorizations[1].Scopes!);

        // Act
        var result = await store.FindAsync(
            authorizations[1].Subject!,
            authorizations[1].ApplicationId.ToString(),
            authorizations[1].Status!,
            authorizations[1].Type!,
            ImmutableArray.Create("invalid-scope"), default).ToListAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task FindByApplicationIdAsync_WithNullIdentifier_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "identifier",
            () => store.FindByApplicationIdAsync(null!, default).ToListAsync().AsTask());
    }

    [Fact]
    public async Task FindByApplicationIdAsync_WithIdentifier_ReturnsAuthorization()
    {
        // Arrange
        var authorizations = new OpenIddictLiteDBAuthorizationFaker().Generate(3);
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder()
            .WithAuthorizations(authorizations)
            .Build();

        // Act
        var result = await store.FindByApplicationIdAsync(authorizations[1].ApplicationId.ToString(), default).ToListAsync();

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.Equal(authorizations[1].Id, result[0].Id));
    }

    [Fact]
    public async Task FindByIdAsync_WithNullIdentifier_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder().Build();
        
        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "identifier",
            () => store.FindByIdAsync(null!, default).AsTask());
    }

    [Fact]
    public async Task FindByIdAsync_WithIdentifier_ReturnsAuthorization()
    {
        // Arrange
        var authorizations = new OpenIddictLiteDBAuthorizationFaker().Generate(3);
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder()
            .WithAuthorizations(authorizations)
            .Build();

        // Act
        var result = await store.FindByIdAsync(authorizations[1].Id.ToString(), default);

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.Equal(authorizations[1].Id, result!.Id));
    }

    [Fact]
    public async Task FindBySubjectAsync_WithNullSubject_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "subject",
            () => store.FindBySubjectAsync(null!, default).ToListAsync().AsTask());
    }

    [Fact]
    public async Task FindBySubjectAsync_WithSubject_ReturnsAuthorization()
    {
        // Arrange
        var authorizations = new OpenIddictLiteDBAuthorizationFaker().Generate(3);
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder()
            .WithAuthorizations(authorizations)
            .Build();

        // Act
        var result = await store.FindBySubjectAsync(authorizations[1].Subject!, default).ToListAsync();

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.Equal(authorizations[1].Id, result[0].Id));
    }

    [Fact]
    public async Task GetAsync_WithNullQuery_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder().Build();

        Func<IQueryable<object>, object, IQueryable<object>>? query = null;

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            "query",
            () => store.GetAsync(query!, null, default).AsTask());
    }

    [Fact]
    public async Task GetAsync_WithQuery_ReturnsAppropriateResult()
    {
        // Arrange
        var authorizations = new OpenIddictLiteDBAuthorizationFaker().Generate(3);
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder()
            .WithAuthorizations(authorizations)
            .Build();

        var query = (IQueryable<OpenIddictLiteDBAuthorization> query, ObjectId state) =>
            query.Where(x => x.Id == state).Select(x => x.Subject);

        // Act
        var result = await store.GetAsync(query, authorizations[1].Id, default);

        // Assert
        Assert.Equal(authorizations[1].Subject, result);
    }

    [Fact]
    public async Task InstantiateAsync_ReturnsScope()
    {
        // Arrange
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder().Build();

        // Act
        var result = await store.InstantiateAsync(default);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OpenIddictLiteDBAuthorization>(result);
    }

    [Fact]
    public async Task ListAsync_WithCountAndOffset_ReturnsScopes()
    {
        // Arrange
        var authorizations = new OpenIddictLiteDBAuthorizationFaker().Generate(3);
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder()
            .WithAuthorizations(authorizations)
            .Build();

        // Act
        var result = await store.ListAsync(1, 0, default).ToListAsync();

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.Single(result));
    }

    [Fact]
    public async Task ListAsync_WithNullQuery_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder().Build();

        Func<IQueryable<object>, object, IQueryable<object>>? query = null;

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            "query",
            () => store.ListAsync(query!, null, default).ToListAsync().AsTask());
    }

    [Fact]
    public async Task ListAsync_WithQuery_ReturnsAppropriateResult()
    {
        // Arrange
        var authorizations = new OpenIddictLiteDBAuthorizationFaker().Generate(3);
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder()
            .WithAuthorizations(authorizations)
            .Build();

        var query = (IQueryable<OpenIddictLiteDBAuthorization> query, object state) =>
            query.Where(x => x.Id == authorizations[0].Id || x.Id == authorizations[1].Id).Select(x => x.Subject);

        // Act
        var result = await store.ListAsync(query!, null, default).ToListAsync();

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.Equal(2, result.Distinct().Count()),
            () => Assert.True(authorizations.Select(x => x.Subject).Intersect(result).Count() == 2));
    }

    [Fact]
    public async Task UpdateAsync_WithNullAuthorization_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            "authorization",
            () => store.UpdateAsync(null!, default).AsTask());
    }

    [Fact]
    public async Task UpdateAsync_WithAuthorization_UpdatesAppropriateAuthorization()
    {
        // Arrange
        var authorizations = new OpenIddictLiteDBAuthorizationFaker().Generate(3);
        var database = new OpenIddictLiteDatabase(":memory:");
        var store = new OpenIddictLiteDBAuthorizationStoreBuilder()
            .WithAuthorizations(authorizations)
            .WithDatabase(database)
            .Build();

        authorizations[1].Subject = "My Fabrikam";

        // Act
        await store.UpdateAsync(authorizations[1], default);

        // Assert
        var result = database.Authorizations().FindById(authorizations[1].Id);
        Assert.Equal("My Fabrikam", result.Subject);
    }
}