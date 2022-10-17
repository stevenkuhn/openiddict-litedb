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
    public async Task CountAync_WithNullQuery_ThrowsException()
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
    public async Task CountAsync_WithQuery_ReturnsCorrespondingApplicationCount()
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
    public async Task CreateAsync_WithNullApplication_ThrowsException()
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
    public async Task DeleteAsync_WithNullApplication_ThrowsException()
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

    [Fact]
    public async Task DeleteAsync_WithConcurrencyTokenChange_ThrowsException()
    {
        // Arrange
        var application = new ApplicationFaker().Generate();
        var database = new DatabaseBuilder()
            .WithApplication(application)
            .Build();

        var store = new ApplicationStoreBuilder(database).Build();
        application.ConcurrencyToken = Guid.NewGuid().ToString();

        // Act
        var exception = await Assert.ThrowsAsync<OpenIddictExceptions.ConcurrencyException>(
            () => store.DeleteAsync(application, default).AsTask());

        // Assert
        await Verify(exception.Message, database);
    }

    [Fact]
    public async Task DeleteAsync_RemovesAuthorizationsAndTokensFromDatabase()
    {
        // Arrange
        var application = new ApplicationFaker().Generate();
        var database = new DatabaseBuilder()
            .WithApplication(application, 
                includeAuthorizations: true, 
                includeTokens: true)
            .WithApplications(2, 
                includeAuthorizations: true,
                includeTokens: true)
            .Build();

        var store = new ApplicationStoreBuilder(database).Build();

        // Act
        await store.DeleteAsync(application, default);

        // Assert
        var result = await Verify(database);
        Assert.DoesNotContain($"client_id: {application.ClientId}", result.Text);
        Assert.DoesNotContain($"application_id: {application.Id}", result.Text);
    }

    [Fact]
    public async Task FindByClientIdAsync_WithNullIdentifier_ThrowsException()
    {
        // Arrange
        var database = new DatabaseBuilder().Build();
        var store = new ApplicationStoreBuilder(database).Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "identifier",
            () => store.FindByClientIdAsync(null!, default).AsTask());
    }

    [Fact]
    public async Task FindByClientIdAsync_WithIdentifier_ReturnsApplication()
    {
        // Arrange
        var application = new ApplicationFaker().Generate();
        var database = new DatabaseBuilder()
            .WithApplication()
            .WithApplication(application)
            .WithApplication()
            .Build();
        var store = new ApplicationStoreBuilder(database).Build();

        // Act
        var result = await store.FindByClientIdAsync(application.ClientId!, default);

        // Assert
        await Verify(result, database);
    }
    
    [Fact]
    public async Task FindByIdAsync_WithNullIdentifier_ThrowsException()
    {
        // Arrange
        var database = new DatabaseBuilder().Build();
        var store = new ApplicationStoreBuilder(database).Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "identifier",
            () => store.FindByIdAsync(null!, default).AsTask());
    }

    [Fact]
    public async Task FindByIdAsyncAsync_WithIdentifier_ReturnsApplication()
    {
        // Arrange
        var application = new ApplicationFaker().Generate();
        var database = new DatabaseBuilder()
            .WithApplication()
            .WithApplication(application)
            .WithApplication()
            .Build();
        var store = new ApplicationStoreBuilder(database).Build();

        // Act
        var result = await store.FindByIdAsync(application.Id.ToString(), default);

        // Assert
        await Verify(result, database);
    }

    [Fact]
    public async Task FindByPostLogoutRedirectUriAsync_WithNullAddress_ThrowsException()
    {
        // Arrange
        var database = new DatabaseBuilder().Build();
        var store = new ApplicationStoreBuilder(database).Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "address",
            () => store.FindByPostLogoutRedirectUriAsync(null!, default).ToListAsync().AsTask());
    }

    [Fact]
    public async Task FindByPostLogoutRedirectUriAsync_WithAddress_ReturnsApplication()
    {
        // Arrange
        var application = new ApplicationFaker()
            .RuleFor(x => x.PostLogoutRedirectUris, new[] { "https://www.fabrikam.com/path1" }.ToImmutableArray())
            .Generate();
        var database = new DatabaseBuilder()
            .WithApplication()
            .WithApplication(application)
            .WithApplication()
            .Build();
        var store = new ApplicationStoreBuilder(database).Build();

        // Act
        var result = await store.FindByPostLogoutRedirectUriAsync("https://www.fabrikam.com/path1", default).ToListAsync();

        // Assert
        await Verify(result, database);
    }

    [Fact]
    public async Task FindByRedirectUriAsync_WithNullAddress_ThrowsException()
    {
        // Arrange
        var database = new DatabaseBuilder().Build();
        var store = new ApplicationStoreBuilder(database).Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "address",
            () => store.FindByRedirectUriAsync(null!, default).ToListAsync().AsTask());
    }

    [Fact]
    public async Task FindByRedirectUriAsync_WithAddress_ReturnsApplication()
    {
        // Arrange
        var application = new ApplicationFaker()
            .RuleFor(x => x.RedirectUris, new[] { "https://www.fabrikam.com/path1" }.ToImmutableArray())
            .Generate();
        var database = new DatabaseBuilder()
            .WithApplication()
            .WithApplication(application)
            .WithApplication()
            .Build();
        var store = new ApplicationStoreBuilder(database).Build();

        // Act
        var result = await store.FindByRedirectUriAsync("https://www.fabrikam.com/path1", default).ToListAsync();

        // Assert
        await Verify(result, database);
    }

    [Fact]
    public async Task GetAsync_WithNullQuery_ThrowsException()
    {
        // Arrange
        var database = new DatabaseBuilder().Build();
        var store = new ApplicationStoreBuilder(database).Build();

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
        var application = new ApplicationFaker().Generate();
        var database = new DatabaseBuilder()
            .WithApplication()
            .WithApplication(application)
            .WithApplication()
            .Build();
        var store = new ApplicationStoreBuilder(database).Build();

        var query = (IQueryable<OpenIddictLiteDBApplication> query, ObjectId state) =>
            query.Where(x => x.Id == application.Id).Select(x => x.DisplayName);

        // Act
        var result = await store.GetAsync(query, application.Id, default);

        // Assert
        await Verify(result, database);
    }

    [Fact]
    public async Task InstantiateAsync_ReturnsApplication()
    {
        // Arrange
        var database = new DatabaseBuilder().Build();
        var store = new ApplicationStoreBuilder(database).Build();

        // Act
        var result = await store.InstantiateAsync(default);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OpenIddictLiteDBApplication>(result);
    }

    [Fact]
    public async Task ListAsync_WithCountAndOffset_ReturnsApplications()
    {
        // Arrange
        var database = new DatabaseBuilder()
            .WithApplications(3)
            .Build();

        var store = new ApplicationStoreBuilder(database).Build();

        // Act
        var result = await store.ListAsync(1, 0, default).ToListAsync();

        // Assert
        await Verify(result, database);
    }

    [Fact]
    public async Task ListAsync_WithNullQuery_ThrowsException()
    {
        // Arrange
        var database = new DatabaseBuilder().Build();
        var store = new ApplicationStoreBuilder(database).Build();

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
        var application1 = new ApplicationFaker().Generate();
        var application2 = new ApplicationFaker().Generate();
        var database = new DatabaseBuilder()
            .WithApplication()
            .WithApplication(application1)
            .WithApplication(application2)
            .Build();
        var store = new ApplicationStoreBuilder(database).Build();

        var query = (IQueryable<OpenIddictLiteDBApplication> query, object state) =>
            query.Where(x => x.Id == application1.Id || x.Id == application2.Id).Select(x => x.DisplayName);

        // Act
        var result = await store.ListAsync(query!, null, default).ToListAsync();
        
        // Assert
        var verify = await Verify(result, database);
    }

    [Fact]
    public async Task UpdateAsync_WithNullApplication_ThrowsException()
    {
        // Arrange
        var database = new DatabaseBuilder().Build();
        var store = new ApplicationStoreBuilder(database).Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            "application",
            () => store.UpdateAsync(null!, default).AsTask());
    }

    [Fact]
    public async Task UpdateAsync_WithApplication_UpdatesAppropriateApplication()
    {
        // Arrange
        var application = new ApplicationFaker().Generate();
        var database = new DatabaseBuilder()
            .WithApplication(application)
            .Build();
        var store = new ApplicationStoreBuilder(database).Build();

        application.DisplayName = "My Fabrikam";

        // Act
        await store.UpdateAsync(application, default);

        // Assert
        await Verify(database);
    }
}
