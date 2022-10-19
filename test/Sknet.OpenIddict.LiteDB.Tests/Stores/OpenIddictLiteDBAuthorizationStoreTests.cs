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
}