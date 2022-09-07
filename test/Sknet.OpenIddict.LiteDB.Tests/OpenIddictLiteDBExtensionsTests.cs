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

public class OpenIddictLiteDBExtensionsTests
{
    [Fact]
    public void UseLiteDB_ThrowsAnExceptionForNullBuilder()
    {
        // Arrange
        var builder = (OpenIddictCoreBuilder)null!;

        // Act and assert
        var exception = Assert.Throws<ArgumentNullException>(builder.UseLiteDB);

        Assert.Equal("builder", exception.ParamName);
    }

    [Fact]
    public void UseLiteDB_ThrowsAnExceptionForNullConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new OpenIddictCoreBuilder(services);

        // Act and assert
        var exception = Assert.Throws<ArgumentNullException>(() => builder.UseLiteDB(configuration: null!));

        Assert.Equal("configuration", exception.ParamName);
    }

    [Fact]
    public void UseLiteDB_RegistersDefaultEntities()
    {
        // Arrange
        var services = new ServiceCollection().AddOptions();
        var builder = new OpenIddictCoreBuilder(services);

        // Act
        builder.UseLiteDB();

        // Assert
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptionsMonitor<OpenIddictCoreOptions>>().CurrentValue;

        Assert.Equal(typeof(OpenIddictLiteDBApplication), options.DefaultApplicationType);
        Assert.Equal(typeof(OpenIddictLiteDBAuthorization), options.DefaultAuthorizationType);
        Assert.Equal(typeof(OpenIddictLiteDBScope), options.DefaultScopeType);
        Assert.Equal(typeof(OpenIddictLiteDBToken), options.DefaultTokenType);
    }

    [Theory]
    [InlineData(typeof(IOpenIddictApplicationStoreResolver), typeof(OpenIddictLiteDBApplicationStoreResolver))]
    [InlineData(typeof(IOpenIddictAuthorizationStoreResolver), typeof(OpenIddictLiteDBAuthorizationStoreResolver))]
    [InlineData(typeof(IOpenIddictScopeStoreResolver), typeof(OpenIddictLiteDBScopeStoreResolver))]
    [InlineData(typeof(IOpenIddictTokenStoreResolver), typeof(OpenIddictLiteDBTokenStoreResolver))]
    public void UseLiteDB_RegistersLiteDBStoreResolvers(Type serviceType, Type implementationType)
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new OpenIddictCoreBuilder(services);

        // Act
        builder.UseLiteDB();

        // Assert
        Assert.Contains(services, service => service.ServiceType == serviceType &&
                                             service.ImplementationType == implementationType);
    }

    [Theory]
    [InlineData(typeof(OpenIddictLiteDBApplicationStore<>))]
    [InlineData(typeof(OpenIddictLiteDBAuthorizationStore<>))]
    [InlineData(typeof(OpenIddictLiteDBScopeStore<>))]
    [InlineData(typeof(OpenIddictLiteDBTokenStore<>))]
    public void UseLiteDB_RegistersLiteDBStore(Type type)
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new OpenIddictCoreBuilder(services);

        // Act
        builder.UseLiteDB();

        // Assert
        Assert.Contains(services, service => service.ServiceType == type && service.ImplementationType == type);
    }

    [Fact]
    public void UseLiteDB_RegistersLiteDBContext()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new OpenIddictCoreBuilder(services);

        // Act
        builder.UseLiteDB();

        // Assert
        Assert.Contains(services, service => service.Lifetime == ServiceLifetime.Singleton &&
                                             service.ServiceType == typeof(IOpenIddictLiteDBContext) &&
                                             service.ImplementationType == typeof(OpenIddictLiteDBContext));
    }
}