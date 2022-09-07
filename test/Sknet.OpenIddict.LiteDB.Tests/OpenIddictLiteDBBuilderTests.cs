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

public class OpenIddictLiteDBBuilderTests
{
    [Fact]
    public void Constructor_ThrowsAnExceptionForNullServices()
    {
        // Arrange
        var services = (IServiceCollection)null!;

        // Act and assert
        var exception = Assert.Throws<ArgumentNullException>(() => new OpenIddictLiteDBBuilder(services));

        Assert.Equal("services", exception.ParamName);
    }

    [Fact]
    public void ReplaceDefaultApplicationEntity_EntityIsCorrectlySet()
    {
        // Arrange
        var services = CreateServices();
        var builder = CreateBuilder(services);

        // Act
        builder.ReplaceDefaultApplicationEntity<CustomApplication>();

        // Assert
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptionsMonitor<OpenIddictCoreOptions>>().CurrentValue;

        Assert.Equal(typeof(CustomApplication), options.DefaultApplicationType);
    }

    [Fact]
    public void ReplaceDefaultAuthorizationEntity_EntityIsCorrectlySet()
    {
        // Arrange
        var services = CreateServices();
        var builder = CreateBuilder(services);

        // Act
        builder.ReplaceDefaultAuthorizationEntity<CustomAuthorization>();

        // Assert
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptionsMonitor<OpenIddictCoreOptions>>().CurrentValue;

        Assert.Equal(typeof(CustomAuthorization), options.DefaultAuthorizationType);
    }

    [Fact]
    public void ReplaceDefaultScopeEntity_EntityIsCorrectlySet()
    {
        // Arrange
        var services = CreateServices();
        var builder = CreateBuilder(services);

        // Act
        builder.ReplaceDefaultScopeEntity<CustomScope>();

        // Assert
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptionsMonitor<OpenIddictCoreOptions>>().CurrentValue;

        Assert.Equal(typeof(CustomScope), options.DefaultScopeType);
    }

    [Fact]
    public void ReplaceDefaultTokenEntity_EntityIsCorrectlySet()
    {
        // Arrange
        var services = CreateServices();
        var builder = CreateBuilder(services);

        // Act
        builder.ReplaceDefaultTokenEntity<CustomToken>();

        // Assert
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptionsMonitor<OpenIddictCoreOptions>>().CurrentValue;

        Assert.Equal(typeof(CustomToken), options.DefaultTokenType);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void SetApplicationsCollectionName_ThrowsAnExceptionForNullOrEmptyCollectionName(string name)
    {
        // Arrange
        var services = CreateServices();
        var builder = CreateBuilder(services);

        // Act and assert
        var exception = Assert.Throws<ArgumentException>(() => builder.SetApplicationsCollectionName(name));

        Assert.Equal("name", exception.ParamName);
        Assert.StartsWith("The collection name cannot be null or empty.", exception.Message);
    }

    [Fact]
    public void SetApplicationsCollectionName_CollectionNameIsCorrectlySet()
    {
        // Arrange
        var services = CreateServices();
        var builder = CreateBuilder(services);

        // Act
        builder.SetApplicationsCollectionName("custom_collection");

        // Assert
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptionsMonitor<OpenIddictLiteDBOptions>>().CurrentValue;

        Assert.Equal("custom_collection", options.ApplicationsCollectionName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void SetAuthorizationsCollectionName_ThrowsAnExceptionForNullOrEmptyCollectionName(string name)
    {
        // Arrange
        var services = CreateServices();
        var builder = CreateBuilder(services);

        // Act and assert
        var exception = Assert.Throws<ArgumentException>(() => builder.SetAuthorizationsCollectionName(name));

        Assert.Equal("name", exception.ParamName);
        Assert.StartsWith("The collection name cannot be null or empty.", exception.Message);
    }

    [Fact]
    public void SetAuthorizationsCollectionName_CollectionNameIsCorrectlySet()
    {
        // Arrange
        var services = CreateServices();
        var builder = CreateBuilder(services);

        // Act
        builder.SetAuthorizationsCollectionName("custom_collection");

        // Assert
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptionsMonitor<OpenIddictLiteDBOptions>>().CurrentValue;

        Assert.Equal("custom_collection", options.AuthorizationsCollectionName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void SetScopesCollectionName_ThrowsAnExceptionForNullOrEmptyCollectionName(string name)
    {
        // Arrange
        var services = CreateServices();
        var builder = CreateBuilder(services);

        // Act and assert
        var exception = Assert.Throws<ArgumentException>(() => builder.SetScopesCollectionName(name));

        Assert.Equal("name", exception.ParamName);
        Assert.StartsWith("The collection name cannot be null or empty.", exception.Message);
    }

    [Fact]
    public void SetScopesCollectionName_CollectionNameIsCorrectlySet()
    {
        // Arrange
        var services = CreateServices();
        var builder = CreateBuilder(services);

        // Act
        builder.SetScopesCollectionName("custom_collection");

        // Assert
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptionsMonitor<OpenIddictLiteDBOptions>>().CurrentValue;

        Assert.Equal("custom_collection", options.ScopesCollectionName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void SetTokensCollectionName_ThrowsAnExceptionForNullOrEmptyCollectionName(string name)
    {
        // Arrange
        var services = CreateServices();
        var builder = CreateBuilder(services);

        // Act and assert
        var exception = Assert.Throws<ArgumentException>(() => builder.SetTokensCollectionName(name));

        Assert.Equal("name", exception.ParamName);
        Assert.StartsWith("The collection name cannot be null or empty.", exception.Message);
    }

    [Fact]
    public void SetTokensCollectionName_CollectionNameIsCorrectlySet()
    {
        // Arrange
        var services = CreateServices();
        var builder = CreateBuilder(services);

        // Act
        builder.SetTokensCollectionName("custom_collection");

        // Assert
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptionsMonitor<OpenIddictLiteDBOptions>>().CurrentValue;

        Assert.Equal("custom_collection", options.TokensCollectionName);
    }

    [Fact]
    public void UseDatabase_ThrowsAnExceptionForNullDatabase()
    {
        // Arrange
        var services = CreateServices();
        var builder = CreateBuilder(services);

        // Act and assert
        var exception = Assert.Throws<ArgumentNullException>(delegate
        {
            return builder.UseDatabase(database: null!);
        });

        Assert.Equal("database", exception.ParamName);
    }

    [Fact]
    public void UseDatabase_SetsDatabaseInOptions()
    {
        // Arrange
        var services = CreateServices();
        var builder = CreateBuilder(services);
        var database = Mock.Of<ILiteDatabase>();

        // Act
        builder.UseDatabase(database);

        // Assert
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptionsMonitor<OpenIddictLiteDBOptions>>().CurrentValue;

        Assert.Equal(database, options.Database);
    }

    private static OpenIddictLiteDBBuilder CreateBuilder(IServiceCollection services)
        => services.AddOpenIddict().AddCore().UseLiteDB();

    private static IServiceCollection CreateServices()
    {
        var services = new ServiceCollection();
        services.AddOptions();

        return services;
    }

    public class CustomApplication : OpenIddictLiteDBApplication { }
    public class CustomAuthorization : OpenIddictLiteDBAuthorization { }
    public class CustomScope : OpenIddictLiteDBScope { }
    public class CustomToken : OpenIddictLiteDBToken { }
}