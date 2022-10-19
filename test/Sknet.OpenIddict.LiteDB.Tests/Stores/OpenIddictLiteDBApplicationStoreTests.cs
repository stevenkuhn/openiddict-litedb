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

public class OpenIddictLiteDBApplicationStoreTests
{
    [Fact]
    public async Task CountAsync_WithZeroItems_ReturnsZero()
    {
        // Arrange
        var store = new OpenIddictLiteDBApplicationStoreBuilder().Build();

        // Act
        var result = await store.CountAsync(default);

        // Assert
        Assert.Equal(0, result);

    }

    [Fact]
    public async Task CountAsync_WithThreeItems_ReturnsThree()
    {
        // Arrange
        var applications = new OpenIddictLiteDBApplicationFaker().Generate(3);
        var store = new OpenIddictLiteDBApplicationStoreBuilder()
            .WithApplications(applications)
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
    public async Task CountAsync_WithQuery_ReturnsApplicationCount()
    {
        // Arrange
        var applications = new OpenIddictLiteDBApplicationFaker().Generate(3);
        var store = new OpenIddictLiteDBApplicationStoreBuilder()
            .WithApplications(applications)
            .Build();

        // Act
        var result = await store.CountAsync(
            query => query.Where(a => a.ClientId == applications[1].ClientId),
            default);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task CreateAsync_WithNullApplication_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBApplicationStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            "application",
            () => store.CreateAsync(null!, default).AsTask());
    }

    [Fact]
    public async Task CreateAsync_AddsApplicationToDatabase()
    {
        // Arrange
        var database = new OpenIddictLiteDatabase(":memory:");
        var store = new OpenIddictLiteDBApplicationStoreBuilder()
            .WithDatabase(database)
            .Build();
        var application = new OpenIddictLiteDBApplicationFaker().Generate();

        // Act
        await store.CreateAsync(application, default);

        // Assert
        var result = database.Applications().FindById(application.Id);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task DeleteAsync_WithNullApplication_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBApplicationStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            "application",
            () => store.DeleteAsync(null!, default).AsTask());
    }

    [Fact]
    public async Task DeleteAsync_RemovesApplicationFromDatabase()
    {
        // Arrange
        var applications = new OpenIddictLiteDBApplicationFaker().Generate(2);
        var database = new OpenIddictLiteDatabase(":memory:");
        var store = new OpenIddictLiteDBApplicationStoreBuilder()
            .WithApplications(applications)
            .WithDatabase(database)
            .Build();

        // Act
        await store.DeleteAsync(applications[0], default);

        // Assert
        var result = database.Applications().FindById(applications[0].Id);
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WithConcurrencyTokenChange_ThrowsException()
    {
        // Arrange
        var application = new OpenIddictLiteDBApplicationFaker().Generate();
        var store = new OpenIddictLiteDBApplicationStoreBuilder()
            .WithApplications(application)
            .Build();

        application.ConcurrencyToken = Guid.NewGuid().ToString();

        // Act/Assert
        await Assert.ThrowsAsync<OpenIddictExceptions.ConcurrencyException>(
            () => store.DeleteAsync(application, default).AsTask());
    }

    [Fact]
    public async Task DeleteAsync_RemovesAuthorizationsAndTokensFromDatabase()
    {
        // Arrange
        var applications = new OpenIddictLiteDBApplicationFaker().Generate(2);
        var authorizations = new OpenIddictLiteDBAuthorizationFaker()
            .RuleFor(x => x.ApplicationId, f => applications[f.IndexFaker % 2].Id)
            .Generate(2);
        var tokens = new OpenIddictLiteDBTokenFaker()
            .RuleFor(x => x.ApplicationId, f => authorizations[f.IndexFaker % 2].ApplicationId)
            .RuleFor(x => x.AuthorizationId, f => authorizations[f.IndexFaker % 2].Id)
            .Generate(2);
        var database = new OpenIddictLiteDatabase(":memory:");
        var store = new OpenIddictLiteDBApplicationStoreBuilder()
            .WithApplications(applications)
            .WithAuthorizations(authorizations)
            .WithTokens(tokens)
            .WithDatabase(database)
            .Build();

        // Assert
        var applicationId = applications[0].Id;
        var authorizationsResult = database.Authorizations().Find(x => x.ApplicationId == applicationId);
        var tokensResult = database.Tokens().Find(x => x.ApplicationId == applicationId);
        Assert.Multiple(
            () => Assert.NotEmpty(authorizationsResult),
            () => Assert.NotEmpty(tokensResult));

        // Act
        await store.DeleteAsync(applications[0], default);

        // Assert
        authorizationsResult = database.Authorizations().Find(x => x.ApplicationId == applicationId);
        tokensResult = database.Tokens().Find(x => x.ApplicationId == applicationId);
        Assert.Multiple(
            () => Assert.Empty(authorizationsResult),
            () => Assert.Empty(tokensResult));
    }

    [Fact]
    public async Task FindByClientIdAsync_WithNullIdentifier_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBApplicationStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "identifier",
            () => store.FindByClientIdAsync(null!, default).AsTask());
    }

    [Fact]
    public async Task FindByClientIdAsync_WithIdentifier_ReturnsApplication()
    {
        // Arrange
        var applications = new OpenIddictLiteDBApplicationFaker().Generate(3);
        var store = new OpenIddictLiteDBApplicationStoreBuilder()
            .WithApplications(applications)
            .Build();

        // Act
        var result = await store.FindByClientIdAsync(applications[1].ClientId!, default);

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.Equal(applications[1].Id, result!.Id));
    }

    [Fact]
    public async Task FindByIdAsync_WithNullIdentifier_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBApplicationStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "identifier",
            () => store.FindByIdAsync(null!, default).AsTask());
    }

    [Fact]
    public async Task FindByIdAsync_WithIdentifier_ReturnsApplication()
    {
        // Arrange
        var applications = new OpenIddictLiteDBApplicationFaker().Generate(3);
        var store = new OpenIddictLiteDBApplicationStoreBuilder()
            .WithApplications(applications)
            .Build();

        // Act
        var result = await store.FindByIdAsync(applications[1].Id.ToString(), default);

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.Equal(applications[1].Id, result!.Id));
    }

    [Fact]
    public async Task FindByPostLogoutRedirectUriAsync_WithNullAddress_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBApplicationStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "address",
            () => store.FindByPostLogoutRedirectUriAsync(null!, default).ToListAsync().AsTask());
    }

    [Fact]
    public async Task FindByPostLogoutRedirectUriAsync_WithAddress_ReturnsApplication()
    {
        // Arrange
        var applications = new OpenIddictLiteDBApplicationFaker().Generate(3);
        var store = new OpenIddictLiteDBApplicationStoreBuilder()
            .WithApplications(applications)
            .Build();

        // Assert
        Assert.NotNull(applications[2].PostLogoutRedirectUris);
        Assert.NotEmpty(applications[2].PostLogoutRedirectUris!);

        // Act
        var result = await store.FindByPostLogoutRedirectUriAsync(applications[2].PostLogoutRedirectUris!.Value[0], default).ToListAsync();

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.Equal(applications[2].Id, result[0].Id));
    }

    [Fact]
    public async Task FindByRedirectUriAsync_WithNullAddress_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBApplicationStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
        "address",
        () => store.FindByRedirectUriAsync(null!, default).ToListAsync().AsTask());
    }

    [Fact]
    public async Task FindByRedirectUriAsync_WithAddress_ReturnsApplication()
    {
        // Arrange
        var applications = new OpenIddictLiteDBApplicationFaker().Generate(3);
        var store = new OpenIddictLiteDBApplicationStoreBuilder()
            .WithApplications(applications)
            .Build();

        // Assert
        Assert.NotNull(applications[2].RedirectUris);
        Assert.NotEmpty(applications[2].RedirectUris!);

        // Act
        var result = await store.FindByRedirectUriAsync(applications[2].RedirectUris!.Value[0], default).ToListAsync();

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.Equal(applications[2].Id, result[0].Id));
    }

    [Fact]
    public async Task GetAsync_WithNullQuery_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBApplicationStoreBuilder().Build();

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
        var applications = new OpenIddictLiteDBApplicationFaker().Generate(3);
        var store = new OpenIddictLiteDBApplicationStoreBuilder()
            .WithApplications(applications)
            .Build();

        var query = (IQueryable<OpenIddictLiteDBApplication> query, ObjectId state) =>
            query.Where(x => x.Id == state).Select(x => x.DisplayName);

        // Act
        var result = await store.GetAsync(query, applications[1].Id, default);

        // Assert
        Assert.Equal(applications[1].DisplayName, result);
    }

    [Fact]
    public async Task InstantiateAsync_ReturnsApplication()
    {
        // Arrange
        var store = new OpenIddictLiteDBApplicationStoreBuilder().Build();

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
        var applications = new OpenIddictLiteDBApplicationFaker().Generate(3);
        var store = new OpenIddictLiteDBApplicationStoreBuilder()
            .WithApplications(applications)
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
        var store = new OpenIddictLiteDBApplicationStoreBuilder().Build();

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
        var applications = new OpenIddictLiteDBApplicationFaker().Generate(3);
        var store = new OpenIddictLiteDBApplicationStoreBuilder()
            .WithApplications(applications)
            .Build();

        var query = (IQueryable<OpenIddictLiteDBApplication> query, object state) =>
            query.Where(x => x.Id == applications[0].Id || x.Id == applications[1].Id).Select(x => x.DisplayName);

        // Act
        var result = await store.ListAsync(query!, null, default).ToListAsync();

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.Equal(2, result.Distinct().Count()),
            () => Assert.True(applications.Select(x => x.DisplayName).Intersect(result).Count() == 2));
    }

    [Fact]
    public async Task UpdateAsync_WithNullApplication_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBApplicationStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            "application",
            () => store.UpdateAsync(null!, default).AsTask());
    }

    [Fact]
    public async Task UpdateAsync_WithApplication_UpdatesAppropriateApplication()
    {
        // Arrange
        var applications = new OpenIddictLiteDBApplicationFaker().Generate(3);
        var database = new OpenIddictLiteDatabase(":memory:");
        var store = new OpenIddictLiteDBApplicationStoreBuilder()
            .WithApplications(applications)
            .WithDatabase(database)
            .Build();

        applications[1].DisplayName = "My Fabrikam";

        // Act
        await store.UpdateAsync(applications[1], default);

        // Assert
        var result = database.Applications().FindById(applications[1].Id);
        Assert.Equal("My Fabrikam", result.DisplayName);
    }
}