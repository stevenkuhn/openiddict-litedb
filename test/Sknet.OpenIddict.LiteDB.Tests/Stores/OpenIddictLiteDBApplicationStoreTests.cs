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

public class OpenIddictLiteDBApplicationStoreTests
{
    [Fact]
    public async Task CountAsync_ReturnsCorrespondingApplicationCount()
    {
        // Arrange
        var database = new DatabaseBuilder()
            .WithApplication("client-id-1")
            .WithApplication("client-id-2")
            .WithApplication("client-id-3")
            .Build();

        var store = new ApplicationStoreBuilder(database).Build();

        // Act
        var result = await store.CountAsync(default);

        // Assert
        Assert.Equal(3, result);
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
}