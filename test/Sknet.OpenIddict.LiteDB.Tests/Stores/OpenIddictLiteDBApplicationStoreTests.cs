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
        var database = new DatabaseBuilder()
            .WithApplications(3)
            .Build();

        var store = new ApplicationStoreBuilder(database).Build();

        // Act
        var result = await store.CountAsync(default);

        // Assert
        await Verify(result, database);
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
            .WithApplications(2)
            .WithApplication("client-id-test")
            .Build();

        var store = new ApplicationStoreBuilder(database).Build();

        // Act
        var result = await store.CountAsync(
            query => query.Where(a => a.ClientId == "client-id-test"),
            default);

        // Assert
        await Verify(result, database);
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
        var application = new ApplicationFaker().Generate();

        // Act
        await store.CreateAsync(application, default);

        // Assert
        await Verify(database);
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
        var application = new ApplicationFaker().Generate();
        var database = new DatabaseBuilder()
            .WithApplications(2)
            .WithApplication(application)
            .Build();
        
        var store = new ApplicationStoreBuilder(database).Build();

        // Act
        await store.DeleteAsync(application, default);

        // Assert
        var result = await Verify(database);
        Assert.DoesNotContain($"client_id: {application.ClientId}", result.Text);
    }

    // DeleteAsync with Concurrency issue
    // DeleteAsync - Deletes authorizations
    // DeleteAsync - Deletes tokens
}
