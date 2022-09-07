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

public class OpenIddictLiteDBApplicationStoreResolverTests
{
    [Fact]
    public void Get_ReturnsCustomStoreCorrespondingToTheSpecifiedTypeWhenAvailable()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton(Mock.Of<IOpenIddictApplicationStore<CustomApplication>>());

        var provider = services.BuildServiceProvider();
        var resolver = new OpenIddictLiteDBApplicationStoreResolver(provider);

        // Act and assert
        Assert.NotNull(resolver.Get<CustomApplication>());
    }

    [Fact]
    public void Get_ThrowsAnExceptionForInvalidEntityType()
    {
        // Arrange
        var services = new ServiceCollection();

        var provider = services.BuildServiceProvider();
        var resolver = new OpenIddictLiteDBApplicationStoreResolver(provider);

        // Act and assert
        var exception = Assert.Throws<InvalidOperationException>(resolver.Get<CustomApplication>);

        Assert.Equal("The specified application type is not compatible with the LiteDB stores.\r\nWhen enabling the LiteDB stores, make sure you use the built-in 'OpenIddictLiteDBApplication' entity or a custom entity that inherits from the 'OpenIddictLiteDBApplication' entity.", exception.Message);
    }

    [Fact]
    public void Get_ReturnsDefaultStoreCorrespondingToTheSpecifiedTypeWhenAvailable()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton(Mock.Of<IOpenIddictApplicationStore<CustomApplication>>());
        services.AddSingleton(CreateStore());

        var provider = services.BuildServiceProvider();
        var resolver = new OpenIddictLiteDBApplicationStoreResolver(provider);

        // Act and assert
        Assert.NotNull(resolver.Get<MyApplication>());
    }

    private static OpenIddictLiteDBApplicationStore<MyApplication> CreateStore()
        => new Mock<OpenIddictLiteDBApplicationStore<MyApplication>>(
            Mock.Of<IOpenIddictLiteDBContext>(),
            Mock.Of<IOptionsMonitor<OpenIddictLiteDBOptions>>()).Object;

    public class CustomApplication { }

    public class MyApplication : OpenIddictLiteDBApplication { }
}
