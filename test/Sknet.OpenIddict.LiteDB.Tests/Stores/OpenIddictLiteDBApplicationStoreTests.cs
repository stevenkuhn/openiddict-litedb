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
namespace Sknet.OpenIddict.LiteDB.Tests;

[UsesVerify]
public class OpenIddictLiteDBApplicationStoreTests
{
    public OpenIddictLiteDBApplicationStoreTests()
    {
        Randomizer.Seed = new Random(452856);
    }

    [Fact]
    public async Task CountAsync_ReturnsCorrespondingApplicationCount()
    {
        // Arrange
        //var database = new DatabaseBuilder()
        //    .WithApplication("client-id-1")
        //    .WithApplication("client-id-2")
        //    .WithApplication("client-id-3")
        //    .Build();  

        var store = new ApplicationStoreBuilder(database).Build();

        // Act
        var result = await store.CountAsync(default);

        // Assert
        await Verify(result);
    }

    [Fact]
    public async Task CountAyncWithNullQuery_ThrowsException()
    {
        // Arrange
        var database = new DatabaseBuilder().Build();
        var store = new ApplicationStoreBuilder(database).Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            "query",
            () => store.CountAsync<object>(null!, default).AsTask());
    }

    [Fact]
    public async Task CountAsyncWithQuery_ReturnsCorrespondingApplicationCount()
    {
        // Arrange
        var database = new DatabaseBuilder()
            .WithApplication("client-id-1")
            .WithApplication("client-id-2")
            .WithApplication("client-id-3")
            .Build();

        var store = new ApplicationStoreBuilder(database).Build();

        // Act
        var result = await store.CountAsync(
            query => query.Where(a => a.ClientId  == "client-id-2"),
            default);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task CreateAsyncWithNullApplication_ThrowsException()
    {
        // Arrange
        var database = new DatabaseBuilder().Build();
        var store = new ApplicationStoreBuilder(database).Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            "application",
            () => store.CreateAsync(null!, default).AsTask());
    }

    [Fact]
    public async Task CreateAsync_AddsApplicationToDatabase()
    {
        // Arrange
        var database = new DatabaseBuilder().Build();
        var store = new ApplicationStoreBuilder(database).Build();
        var expectedApp = new OpenIddictLiteDBApplication();

        // Act
        await store.CreateAsync(expectedApp, default);
        var actualApp = database
            .GetCollection<OpenIddictLiteDBApplication>("openiddict_applications")
            .FindById(expectedApp.Id);

        // Assert
        Assert.NotNull(actualApp);
        Assert.Equal(expectedApp.Id, actualApp.Id);
    }

    [Fact]
    public async Task DeleteAsyncWithNullApplication_ThrowsException()
    {
        // Arrange
        var database = new DatabaseBuilder().Build();
        var store = new ApplicationStoreBuilder(database).Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            "application",
            () => store.DeleteAsync(null!, default).AsTask());
    }

    [Fact]
    public async Task DeleteAsync_RemovesApplicationFromDatabase()
    {
        // Arrange
        var app = new OpenIddictLiteDBApplication() { ClientId = "client-id-2" };
        var database = new DatabaseBuilder()
            .WithApplication("client-id-1")
            .WithApplication(app)
            .WithApplication("client-id-3")
            .Build();

        var store = new ApplicationStoreBuilder(database).Build();

        // Act
        await store.DeleteAsync(app, default);

        // Assert
        var collection = database.GetCollection<OpenIddictLiteDBApplication>("openiddict_applications");
        var count = collection.Count();
        Assert.Equal(2, count);
        Assert.True(collection.FindAll().All(x => x.ClientId != "client-id-2"));
    }

    // DeleteAsync with Concurrency issue
    // DeleteAsync - Deletes authorizations
    // DeleteAsync - Deletes tokens
}