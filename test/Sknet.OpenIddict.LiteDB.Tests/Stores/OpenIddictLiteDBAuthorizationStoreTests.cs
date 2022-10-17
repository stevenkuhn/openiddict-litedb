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

[UsesVerify]
public class OpenIddictLiteDBAuthorizationStoreTests
{
    public OpenIddictLiteDBAuthorizationStoreTests()
    {
        Randomizer.Seed = new Random(452856);
    }

    [Fact]
    public async Task CountAsync_ReturnsCorrespondingAuthorizationCount()
    {
        // Arrange
        var database = new DatabaseBuilder()
            .WithAuthorizations(3)
            .Build();

        var store = new AuthorizationStoreBuilder(database).Build();

        // Act
        var result = await store.CountAsync(default);

        // Assert
        await Verify(result, database);
    }

    [Fact]
    public async Task CountAync_WithNullQuery_ThrowsException()
    {
        // Arrange
        var database = new DatabaseBuilder().Build();
        var store = new AuthorizationStoreBuilder(database).Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            "query",
            () => store.CountAsync<object>(null!, default).AsTask());
    }

    [Fact]
    public async Task CountAsync_WithQuery_ReturnsCorrespondingAuthorizationCount()
    {
        // Arrange
        var authorization = new AuthorizationFaker().UseSeed(3484).Generate();
        var database = new DatabaseBuilder()
            .WithAuthorization()
            .WithAuthorization(authorization)
            .Build();

        var store = new AuthorizationStoreBuilder(database).Build();

        // Act
        var result = await store.CountAsync(
            query => query.Where(a => a.ApplicationId == authorization.ApplicationId),
            default);

        // Assert
        await Verify(result, database);
    }

    [Fact]
    public async Task CreateAsync_AddsAuthorizationToDatabase()
    {
        // Arrange
        var database = new DatabaseBuilder().Build();
        var store = new AuthorizationStoreBuilder(database).Build();
        var authorization = new AuthorizationFaker().UseSeed(3484).Generate();

        // Act
        await store.CreateAsync(authorization, default);

        // Assert
        await Verify(database);
    }

    [Fact]
    public async Task DeleteAsync_WithNullAuthorization_ThrowsException()
    {
        // Arrange
        var database = new DatabaseBuilder().Build();
        var store = new AuthorizationStoreBuilder(database).Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            "authorization",
            () => store.DeleteAsync(null!, default).AsTask());
    }

    [Fact]
    public async Task DeleteAsync_RemovesAuthorizationFromDatabase()
    {
        // Arrange
        var authorization = new AuthorizationFaker().UseSeed(3484).Generate();
        var database = new DatabaseBuilder()
            .WithAuthorizations(2)
            .WithAuthorization(authorization)
            .Build();

        var store = new AuthorizationStoreBuilder(database).Build();

        // Act
        await store.DeleteAsync(authorization, default);

        // Assert
        var result = await Verify(database);
        Assert.DoesNotContain($"application_id: {authorization.ApplicationId}", result.Text);
    }

    [Fact]
    public async Task DeleteAsync_WithConcurrencyTokenChange_ThrowsException()
    {
        // Arrange
        var authorization = new AuthorizationFaker().UseSeed(3484).Generate();
        var database = new DatabaseBuilder()
            .WithAuthorization(authorization)
            .Build();

        var store = new AuthorizationStoreBuilder(database).Build();
        authorization.ConcurrencyToken = Guid.NewGuid().ToString();

        // Act
        var exception = await Assert.ThrowsAsync<OpenIddictExceptions.ConcurrencyException>(
            () => store.DeleteAsync(authorization, default).AsTask());

        // Assert
        await Verify(exception.Message, database);
    }
}
