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

public class OpenIddictLiteDBContextTests
{
    [Fact]
    public async Task GetDatabaseAsync_ThrowsAnExceptionForCanceledToken()
    {
        // Arrange
        var services = new ServiceCollection();
        var provider = services.BuildServiceProvider();

        var options = Mock.Of<IOptionsMonitor<OpenIddictLiteDBOptions>>();
        var token = new CancellationToken(canceled: true);

        var context = new OpenIddictLiteDBContext(options, provider);

        // Act and assert
        var exception = await Assert.ThrowsAsync<TaskCanceledException>(async delegate
        {
            await context.GetDatabaseAsync(token);
        });

        Assert.Equal(token, exception.CancellationToken);
    }

    [Fact]
    public async Task GetDatabaseAsync_PrefersDatabaseRegisteredInOptionsToDatabaseRegisteredInDependencyInjectionContainer()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton(Mock.Of<ILiteDatabase>());

        var provider = services.BuildServiceProvider();

        var database = Mock.Of<ILiteDatabase>();
        var options = Mock.Of<IOptionsMonitor<OpenIddictLiteDBOptions>>(
            mock => mock.CurrentValue == new OpenIddictLiteDBOptions
            {
                Database = database
            });

        var context = new OpenIddictLiteDBContext(options, provider);

        // Act and assert
        Assert.Same(database, await context.GetDatabaseAsync(CancellationToken.None));
    }

    [Fact]
    public async Task GetDatabaseAsync_ThrowsAnExceptionWhenDatabaseCannotBeFound()
    {
        // Arrange
        var services = new ServiceCollection();
        var provider = services.BuildServiceProvider();

        var options = Mock.Of<IOptionsMonitor<OpenIddictLiteDBOptions>>(
            mock => mock.CurrentValue == new OpenIddictLiteDBOptions
            {
                Database = null
            });

        var context = new OpenIddictLiteDBContext(options, provider);

        // Act and assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async delegate
        {
            await context.GetDatabaseAsync(CancellationToken.None);
        });

        Assert.Equal("No suitable LiteDB database service can be found.\r\nTo configure the OpenIddict LiteDB stores to use a specific database, use 'services.AddOpenIddict().AddCore().UseLiteDB().UseDatabase()' or register an 'ILiteDatabase' in the dependency injection container in 'ConfigureServices()'.", exception.Message);
    }

    [Fact]
    public async Task GetDatabaseAsync_UsesDatabaseRegisteredInDependencyInjectionContainer()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton(Mock.Of<ILiteDatabase>());

        var database = Mock.Of<ILiteDatabase>();
        services.AddSingleton(database);

        var provider = services.BuildServiceProvider();

        var options = Mock.Of<IOptionsMonitor<OpenIddictLiteDBOptions>>(
            mock => mock.CurrentValue == new OpenIddictLiteDBOptions
            {
                Database = null
            });

        var context = new OpenIddictLiteDBContext(options, provider);

        // Act and assert
        Assert.Same(database, await context.GetDatabaseAsync(CancellationToken.None));
    }
}