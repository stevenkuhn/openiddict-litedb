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

public class OpenIddictLiteDBScopeStoreTests
{
    [Fact]
    public async Task CountAsync_WithZeroItems_ReturnsZero()
    {
        // Arrange
        var store = new OpenIddictLiteDBScopeStoreBuilder().Build();

        // Act
        var result = await store.CountAsync(default);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task CountAsync_WithThreeItems_ReturnsThree()
    {
        // Arrange
        var scopes = new OpenIddictLiteDBScopeFaker().Generate(3);
        var store = new OpenIddictLiteDBScopeStoreBuilder()
            .WithScopes(scopes)
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
        var store = new OpenIddictLiteDBScopeStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            "query",
            () => store.CountAsync<object>(null!, default).AsTask());
    }

    [Fact]
    public async Task CountAsync_WithQuery_ReturnsScopeCount()
    {
        // Arrange
        var scopes = new OpenIddictLiteDBScopeFaker().Generate(3);
        var store = new OpenIddictLiteDBScopeStoreBuilder()
            .WithScopes(scopes)
            .Build();

        // Act
        var result = await store.CountAsync(
            query => query.Where(s => s.Id == scopes[1].Id),
            default);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task CreateAsync_WithNullScope_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBScopeStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            "scope",
            () => store.CreateAsync(null!, default).AsTask());
    }

    [Fact]
    public async Task CreateAsync_AddsScopeToDatabase()
    {
        // Arrange
        var database = new OpenIddictLiteDatabase(":memory:");
        var store = new OpenIddictLiteDBScopeStoreBuilder()
            .WithDatabase(database)
            .Build();
        var scope = new OpenIddictLiteDBScopeFaker().Generate();
        
        // Act
        await store.CreateAsync(scope, default);

        // Assert
        var result = database.Scopes().FindById(scope.Id);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task DeleteAsync_WithNullScope_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBScopeStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            "scope",
            () => store.DeleteAsync(null!, default).AsTask());
    }

    [Fact]
    public async Task DeleteAsync_RemovesScopeFromDatabase()
    {
        // Arrange
        var scopes = new OpenIddictLiteDBScopeFaker().Generate(2);
        var database = new OpenIddictLiteDatabase(":memory:");
        var store = new OpenIddictLiteDBScopeStoreBuilder()
            .WithScopes(scopes)
            .WithDatabase(database)
            .Build();

        // Act
        await store.DeleteAsync(scopes[0], default);

        // Assert
        var result = database.Scopes().FindById(scopes[0].Id);
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WithConcurrencyTokenChange_ThrowsException()
    {
        // Arrange
        var scope = new OpenIddictLiteDBScopeFaker().Generate();
        var store = new OpenIddictLiteDBScopeStoreBuilder()
            .WithScopes(scope)
            .Build();

        scope.ConcurrencyToken = Guid.NewGuid().ToString();

        // Act/Assert
        await Assert.ThrowsAsync<OpenIddictExceptions.ConcurrencyException>(
            () => store.DeleteAsync(scope, default).AsTask());
    }

    [Fact]
    public async Task FindByIdAsync_WithNullIdentifier_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBScopeStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "identifier",
            () => store.FindByIdAsync(null!, default).AsTask());
    }

    [Fact]
    public async Task FindByIdAsync_WithIdentifier_ReturnsScope()
    {
        // Arrange
        var scopes = new OpenIddictLiteDBScopeFaker().Generate(3);
        var store = new OpenIddictLiteDBScopeStoreBuilder()
            .WithScopes(scopes)
            .Build();

        // Act
        var result = await store.FindByIdAsync(scopes[1].Id.ToString(), default);

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.Equal(scopes[1].Id, result!.Id));
    }

    [Fact]
    public async Task FindByNameAsync_WithNullName_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBScopeStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "name",
            () => store.FindByNameAsync(null!, default).AsTask());
    }

    [Fact]
    public async Task FindByNameAsync_WithName_ReturnsScope()
    {
        // Arrange
        var scopes = new OpenIddictLiteDBScopeFaker().Generate(3);
        var store = new OpenIddictLiteDBScopeStoreBuilder()
            .WithScopes(scopes)
            .Build();

        // Act
        var result = await store.FindByNameAsync(scopes[1].Name!, default);

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.Equal(scopes[1].Id, result!.Id));
    }

    [Fact]
    public async Task FindByNameAsync_WithNullNames_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBScopeStoreBuilder().Build();

        var names = new[] { "" }.ToImmutableArray();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "names",
            () => store.FindByNamesAsync(names, default).ToListAsync().AsTask());
    }

    [Fact]
    public async Task FindByNameAsync_WithNames_ReturnsScope()
    {
        // Arrange
        var scopes = new OpenIddictLiteDBScopeFaker().Generate(3);
        var store = new OpenIddictLiteDBScopeStoreBuilder()
            .WithScopes(scopes)
            .Build();

        var names = new[] { scopes[0].Name!, scopes[1].Name! }.ToImmutableArray();

        // Act
        var result = await store.FindByNamesAsync(names, default).ToListAsync();

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.Equal(2, result.Distinct().Count()),
            () => Assert.Contains(scopes, s => s.Id == result[0].Id),
            () => Assert.Contains(scopes, s => s.Id == result[1].Id));
    }
    
    [Fact]
    public async Task FindByResourceAsync_WithResource_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBScopeStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            "resource",
            () => store.FindByResourceAsync(null!, default).ToListAsync().AsTask());
    }

    [Fact]
    public async Task FindByResourceAsync_WithResource_ReturnsScope()
    {
        // Arrange
        var scopes = new OpenIddictLiteDBScopeFaker().Generate(3);
        var store = new OpenIddictLiteDBScopeStoreBuilder()
            .WithScopes(scopes)
            .Build();

        // Assert
        Assert.NotNull(scopes[1].Resources);
        Assert.NotEmpty(scopes[1].Resources!);

        // Act
        var result = await store.FindByResourceAsync(scopes[1].Resources!.Value[0], default).ToListAsync();

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.Equal(scopes[1].Id, result[1].Id));
    }


    [Fact]
    public async Task GetAsync_WithNullQuery_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBScopeStoreBuilder().Build();

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
        var scopes = new OpenIddictLiteDBScopeFaker().Generate(3);
        var store = new OpenIddictLiteDBScopeStoreBuilder()
            .WithScopes(scopes)
            .Build();

        var query = (IQueryable<OpenIddictLiteDBScope> query, ObjectId state) =>
            query.Where(x => x.Id == state).Select(x => x.DisplayName);

        // Act
        var result = await store.GetAsync(query, scopes[1].Id, default);

        // Assert
        Assert.Equal(scopes[1].DisplayName, result);
    }

    [Fact]
    public async Task InstantiateAsync_ReturnsScope()
    {
        // Arrange
        var store = new OpenIddictLiteDBScopeStoreBuilder().Build();

        // Act
        var result = await store.InstantiateAsync(default);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OpenIddictLiteDBScope>(result);
    }

    [Fact]
    public async Task ListAsync_WithCountAndOffset_ReturnsScopes()
    {
        // Arrange
        var scopes = new OpenIddictLiteDBScopeFaker().Generate(3);
        var store = new OpenIddictLiteDBScopeStoreBuilder()
            .WithScopes(scopes)
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
        var store = new OpenIddictLiteDBScopeStoreBuilder().Build();

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
        var scopes = new OpenIddictLiteDBScopeFaker().Generate(3);
        var store = new OpenIddictLiteDBScopeStoreBuilder()
            .WithScopes(scopes)
            .Build();

        var query = (IQueryable<OpenIddictLiteDBScope> query, object state) =>
            query.Where(x => x.Id == scopes[0].Id || x.Id == scopes[1].Id).Select(x => x.DisplayName);

        // Act
        var result = await store.ListAsync(query!, null, default).ToListAsync();

        // Assert
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.Equal(2, result.Distinct().Count()),
            () => Assert.True(scopes.Select(x => x.DisplayName).Intersect(result).Count() == 2));
    }

    [Fact]
    public async Task UpdateAsync_WithNullScope_ThrowsException()
    {
        // Arrange
        var store = new OpenIddictLiteDBScopeStoreBuilder().Build();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            "scope",
            () => store.UpdateAsync(null!, default).AsTask());
    }

    [Fact]
    public async Task UpdateAsync_WithScope_UpdatesAppropriateScope()
    {
        // Arrange
        var scopes = new OpenIddictLiteDBScopeFaker().Generate(3);
        var database = new OpenIddictLiteDatabase(":memory:");
        var store = new OpenIddictLiteDBScopeStoreBuilder()
            .WithScopes(scopes)
            .WithDatabase(database)
            .Build();

        scopes[1].DisplayName = "My Fabrikam";

        // Act
        await store.UpdateAsync(scopes[1], default);

        // Assert
        var result = database.Scopes().FindById(scopes[1].Id);
        Assert.Equal("My Fabrikam", result.DisplayName);
    }
}
