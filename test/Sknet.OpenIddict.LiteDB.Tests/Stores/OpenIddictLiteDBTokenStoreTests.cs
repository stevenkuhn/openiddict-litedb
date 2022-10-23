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

public class OpenIddictLiteDBTokenStoreTests
{
    [Fact]
    public async Task CountAsync_WithZeroItems_ReturnsZero()
    {
        // Arrange
        var store = new OpenIddictLiteDBTokenStoreBuilder().Build();

        // Act
        var result = await store.CountAsync(default);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task CountAsync_WithThreeItems_ReturnsThree()
    {
        // Arrange
        var tokens = new OpenIddictLiteDBTokenFaker().Generate(3);
        var store = new OpenIddictLiteDBTokenStoreBuilder()
            .WithTokens(tokens)
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
        var store = new OpenIddictLiteDBTokenStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            "query",
            () => store.CountAsync<object>(null!, default).AsTask());
    }

    [Fact]
    public async Task CountAsync_WithQuery_ReturnsTokenCount()
    {
        // Arrange
        var tokens = new OpenIddictLiteDBTokenFaker().Generate(3);
        var store = new OpenIddictLiteDBTokenStoreBuilder()
            .WithTokens(tokens)
            .Build();

        // Act
        var result = await store.CountAsync(
            query => query.Where(a => a.Id == tokens[1].Id),
            default);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task CreateAsync_WithNullToken_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBTokenStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            "token",
            () => store.CreateAsync(null!, default).AsTask());
    }

    [Fact]
    public async Task CreateAsync_AddsTokenToDatabase()
    {
        // Arrange
        var database = new OpenIddictLiteDatabase(":memory:");
        var store = new OpenIddictLiteDBTokenStoreBuilder()
            .WithDatabase(database)
            .Build();
        var token = new OpenIddictLiteDBTokenFaker().Generate();

        // Act
        await store.CreateAsync(token, default);

        // Assert
        var result = database.Tokens().FindById(token.Id);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task DeleteAsync_WithNullToken_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBTokenStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            "token",
            () => store.DeleteAsync(null!, default).AsTask());
    }

    [Fact]
    public async Task DeleteAsync_RemovesTokenFromDatabase()
    {
        // Arrange
        var tokens = new OpenIddictLiteDBTokenFaker().Generate(2);
        var database = new OpenIddictLiteDatabase(":memory:");
        var store = new OpenIddictLiteDBTokenStoreBuilder()
            .WithTokens(tokens)
            .WithDatabase(database)
            .Build();

        // Act
        await store.DeleteAsync(tokens[0], default);

        // Assert
        var result = database.Tokens().FindById(tokens[0].Id);
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WithConcurrencyTokenChange_ThrowsException()
    {
        // Arrange
        var token = new OpenIddictLiteDBTokenFaker().Generate();
        var store = new OpenIddictLiteDBTokenStoreBuilder()
            .WithTokens(token)
            .Build();

        token.ConcurrencyToken = Guid.NewGuid().ToString();

        // Act/Assert
        await Assert.ThrowsAsync<OpenIddictExceptions.ConcurrencyException>(
            () => store.DeleteAsync(token, default).AsTask());
    }

    [Fact]
    public async Task FindAsyc_WithNullSubjectAndClient_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBTokenStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "subject",
            () => store.FindAsync(null!, "client", default).ToListAsync().AsTask());
    }

    [Fact]
    public async Task FindAsyc_WithSubjectAndNullClient_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBTokenStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "client",
            () => store.FindAsync("subject", null!, default).ToListAsync().AsTask());
    }

    [Fact]
    public async Task FindAsync_WithSubjectAndClient_ShouldReturnToken()
    {
        // Arrange
        var tokens = new OpenIddictLiteDBTokenFaker().Generate(3);
        var store = new OpenIddictLiteDBTokenStoreBuilder()
            .WithTokens(tokens)
            .Build();

        // Assert
        Assert.NotNull(tokens[1].Subject);
        Assert.NotEqual(ObjectId.Empty, tokens[1].ApplicationId);

        // Act
        var result = await store.FindAsync(tokens[1].Subject!, tokens[1].ApplicationId.ToString(), default).ToListAsync();

        // Assert
        Assert.Multiple(
            () => Assert.Single(result),
            () => Assert.Equal(tokens[1].Id, result[0].Id));
    }

    [Fact]
    public async Task FindAsyc_WithNullSubjectAndClientAndStatus_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBTokenStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "subject",
            () => store.FindAsync(null!, "client", "status", default).ToListAsync().AsTask());
    }

    [Fact]
    public async Task FindAsyc_WithSubjectAndNullClientAndStatus_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBTokenStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "client",
            () => store.FindAsync("subject", null!, "status", default).ToListAsync().AsTask());
    }

    [Fact]
    public async Task FindAsyc_WithSubjectAndClientAndNullStatus_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBTokenStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "status",
            () => store.FindAsync("subject", "client", null!, default).ToListAsync().AsTask());
    }
    
    [Fact]
    public async Task FindAsync_WithSubjectAndClientAndStatus_ShouldReturnToken()
    {
        // Arrange
        var tokens = new OpenIddictLiteDBTokenFaker().Generate(3);
        var store = new OpenIddictLiteDBTokenStoreBuilder()
            .WithTokens(tokens)
            .Build();

        // Assert
        Assert.NotNull(tokens[1].Subject);
        Assert.NotEqual(ObjectId.Empty, tokens[1].ApplicationId);
        Assert.NotNull(tokens[1].Status);

        // Act
        var result = await store.FindAsync(
            tokens[1].Subject!,
            tokens[1].ApplicationId.ToString(),
            tokens[1].Status!, default).ToListAsync();

        // Assert
        Assert.Multiple(
            () => Assert.Single(result),
            () => Assert.Equal(tokens[1].Id, result[0].Id));
    }

    [Fact]
    public async Task FindAsyc_WithNullSubjectAndClientAndStatusAndType_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBTokenStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "subject",
            () => store.FindAsync(null!, "client", "status", "type", default).ToListAsync().AsTask());
    }

    [Fact]
    public async Task FindAsyc_WithSubjectAndNullClientAndStatusAndType_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBTokenStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "client",
            () => store.FindAsync("subject", null!, "status", "type", default).ToListAsync().AsTask());
    }

    [Fact]
    public async Task FindAsyc_WithSubjectAndClientAndNullStatusAndType_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBTokenStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "status",
            () => store.FindAsync("subject", "client", null!, "type", default).ToListAsync().AsTask());
    }

    [Fact]
    public async Task FindAsyc_WithSubjectAndClientAndStatusAndNullType_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBTokenStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "type",
            () => store.FindAsync("subject", "client", "status", null!, default).ToListAsync().AsTask());
    }

    [Fact]
    public async Task FindAsync_WithSubjectAndClientAndStatusAndType_ShouldReturnToken()
    {
        // Arrange
        var tokens = new OpenIddictLiteDBTokenFaker().Generate(3);
        var store = new OpenIddictLiteDBTokenStoreBuilder()
            .WithTokens(tokens)
            .Build();

        // Assert
        Assert.NotNull(tokens[1].Subject);
        Assert.NotEqual(ObjectId.Empty, tokens[1].ApplicationId);
        Assert.NotNull(tokens[1].Status);
        Assert.NotNull(tokens[1].Type);

        // Act
        var result = await store.FindAsync(
            tokens[1].Subject!,
            tokens[1].ApplicationId.ToString(),
            tokens[1].Status!,
            tokens[1].Type!, default).ToListAsync();

        // Assert
        Assert.Multiple(
            () => Assert.Single(result),
            () => Assert.Equal(tokens[1].Id, result[0].Id));
    }

    [Fact]
    public async Task FindByApplicationIdAsync_WithNullIdentifier_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBTokenStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "identifier",
            () => store.FindByApplicationIdAsync(null!, default).ToListAsync().AsTask());
    }

    [Fact]
    public async Task FindByApplicationIdAsync_WithIdentifier_ReturnsToken()
    {
        // Arrange
        var tokens = new OpenIddictLiteDBTokenFaker().Generate(3);
        var store = new OpenIddictLiteDBTokenStoreBuilder()
            .WithTokens(tokens)
            .Build();

        // Act
        var result = await store.FindByApplicationIdAsync(tokens[1].ApplicationId.ToString(), default).ToListAsync();

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.Equal(tokens[1].Id, result[0].Id));
    }

    [Fact]
    public async Task FindByAuthorizationIdAsync_WithNullIdentifier_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBTokenStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "identifier",
            () => store.FindByAuthorizationIdAsync(null!, default).ToListAsync().AsTask());
    }

    [Fact]
    public async Task FindByAuthorizationIdAsync_WithIdentifier_ReturnsToken()
    {
        // Arrange
        var tokens = new OpenIddictLiteDBTokenFaker().Generate(3);
        var store = new OpenIddictLiteDBTokenStoreBuilder()
            .WithTokens(tokens)
            .Build();

        // Act
        var result = await store.FindByAuthorizationIdAsync(tokens[1].AuthorizationId.ToString(), default).ToListAsync();

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.Equal(tokens[1].Id, result[0].Id));
    }

    [Fact]
    public async Task FindByIdAsync_WithNullIdentifier_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBTokenStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "identifier",
            () => store.FindByIdAsync(null!, default).AsTask());
    }

    [Fact]
    public async Task FindByIdAsync_WithIdentifier_ReturnsToken()
    {
        // Arrange
        var tokens = new OpenIddictLiteDBTokenFaker().Generate(3);
        var store = new OpenIddictLiteDBTokenStoreBuilder()
            .WithTokens(tokens)
            .Build();

        // Act
        var result = await store.FindByIdAsync(tokens[1].Id.ToString(), default);

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.Equal(tokens[1].Id, result!.Id));
    }

    [Fact]
    public async Task FindByReferenceIdAsync_WithNullIdentifier_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBTokenStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "identifier",
            () => store.FindByReferenceIdAsync(null!, default).AsTask());
    }

    [Fact]
    public async Task FindByReferenceIdAsync_WithIdentifier_ReturnsToken()
    {
        // Arrange
        var tokens = new OpenIddictLiteDBTokenFaker().Generate(3);
        var store = new OpenIddictLiteDBTokenStoreBuilder()
            .WithTokens(tokens)
            .Build();

        // Act
        var result = await store.FindByReferenceIdAsync(tokens[1].ReferenceId!.ToString(), default);

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.Equal(tokens[1].Id, result!.Id));
    }

    [Fact]
    public async Task FindBySubjectAsync_WithNullSubject_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBTokenStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "subject",
            () => store.FindBySubjectAsync(null!, default).ToListAsync().AsTask());
    }

    [Fact]
    public async Task FindBySubjectAsync_WithSubject_ReturnsToken()
    {
        // Arrange
        var tokens = new OpenIddictLiteDBTokenFaker().Generate(3);
        var store = new OpenIddictLiteDBTokenStoreBuilder()
            .WithTokens(tokens)
            .Build();

        // Act
        var result = await store.FindBySubjectAsync(tokens[1].Subject!, default).ToListAsync();

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.Equal(tokens[1].Id, result[0].Id));
    }

    [Fact]
    public async Task GetAsync_WithNullQuery_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBTokenStoreBuilder().Build();

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
        var tokens = new OpenIddictLiteDBTokenFaker().Generate(3);
        var store = new OpenIddictLiteDBTokenStoreBuilder()
            .WithTokens(tokens)
            .Build();

        var query = (IQueryable<OpenIddictLiteDBToken> query, ObjectId state) =>
            query.Where(x => x.Id == state).Select(x => x.Subject);

        // Act
        var result = await store.GetAsync(query, tokens[1].Id, default);

        // Assert
        Assert.Equal(tokens[1].Subject, result);
    }

    [Fact]
    public async Task InstantiateAsync_ReturnsToken()
    {
        // Arrange
        var store = new OpenIddictLiteDBTokenStoreBuilder().Build();

        // Act
        var result = await store.InstantiateAsync(default);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OpenIddictLiteDBToken>(result);
    }

    [Fact]
    public async Task ListAsync_WithCountAndOffset_ReturnsToken()
    {
        // Arrange
        var tokens = new OpenIddictLiteDBTokenFaker().Generate(3);
        var store = new OpenIddictLiteDBTokenStoreBuilder()
            .WithTokens(tokens)
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
        var store = new OpenIddictLiteDBTokenStoreBuilder().Build();

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
        var tokens = new OpenIddictLiteDBTokenFaker().Generate(3);
        var store = new OpenIddictLiteDBTokenStoreBuilder()
            .WithTokens(tokens)
            .Build();

        var query = (IQueryable<OpenIddictLiteDBToken> query, object state) =>
            query.Where(x => x.Id == tokens[0].Id || x.Id == tokens[1].Id).Select(x => x.Subject);

        // Act
        var result = await store.ListAsync(query!, null, default).ToListAsync();

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.Equal(2, result.Distinct().Count()),
            () => Assert.True(tokens.Select(x => x.Subject).Intersect(result).Count() == 2));
    }

    

    [Fact]
    public async Task UpdateAsync_WithNullToken_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBTokenStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            "token",
            () => store.UpdateAsync(null!, default).AsTask());
    }

    [Fact]
    public async Task UpdateAsync_WithToken_UpdatesAppropriateToken()
    {
        // Arrange
        var tokens = new OpenIddictLiteDBTokenFaker().Generate(3);
        var database = new OpenIddictLiteDatabase(":memory:");
        var store = new OpenIddictLiteDBTokenStoreBuilder()
            .WithTokens(tokens)
            .WithDatabase(database)
            .Build();

        tokens[1].Subject = "My Fabrikam";

        // Act
        await store.UpdateAsync(tokens[1], default);

        // Assert
        var result = database.Tokens().FindById(tokens[1].Id);
        Assert.Equal("My Fabrikam", result.Subject);
    }
}
