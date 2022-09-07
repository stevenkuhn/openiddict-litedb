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
namespace Sknet.OpenIddict.LiteDB;

/// <summary>
/// Exposes a method allowing to resolve a token store.
/// </summary>
public class OpenIddictLiteDBTokenStoreResolver : IOpenIddictTokenStoreResolver
{
    private readonly ConcurrentDictionary<Type, Type> _cache = new ConcurrentDictionary<Type, Type>();
    private readonly IServiceProvider _provider;

    /// <summary>
    /// Creates a new instance of the <see cref="OpenIddictLiteDBTokenStoreResolver"/> class.
    /// </summary>
    public OpenIddictLiteDBTokenStoreResolver(IServiceProvider provider)
        => _provider = provider ?? throw new ArgumentNullException(nameof(provider));

    /// <summary>
    /// Returns a token store compatible with the specified token type or throws an
    /// <see cref="InvalidOperationException"/> if no store can be built using the specified type.
    /// </summary>
    /// <typeparam name="TToken">The type of the Token entity.</typeparam>
    /// <returns>An <see cref="IOpenIddictTokenStore{TToken}"/>.</returns>
    public IOpenIddictTokenStore<TToken> Get<TToken>() where TToken : class
    {
        var store = _provider.GetService<IOpenIddictTokenStore<TToken>>();
        if (store is not null)
        {
            return store;
        }

        var type = _cache.GetOrAdd(typeof(TToken), key =>
        {
            if (!typeof(OpenIddictLiteDBToken).IsAssignableFrom(key))
            {
                throw new InvalidOperationException("The specified token type is not compatible with the LiteDB stores.\r\nWhen enabling the LiteDB stores, make sure you use the built-in 'OpenIddictLiteDBToken' entity or a custom entity that inherits from the 'OpenIddictLiteDBToken' entity.");
            }

            return typeof(OpenIddictLiteDBTokenStore<>).MakeGenericType(key);
        });

        return (IOpenIddictTokenStore<TToken>)_provider.GetRequiredService(type);
    }
}