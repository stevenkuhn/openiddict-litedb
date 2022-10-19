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
}
