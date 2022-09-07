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
/// Exposes a method allowing to resolve a scope store.
/// </summary>
public class OpenIddictLiteDBScopeStoreResolver : IOpenIddictScopeStoreResolver
{
    private readonly ConcurrentDictionary<Type, Type> _cache = new ConcurrentDictionary<Type, Type>();
    private readonly IServiceProvider _provider;

    public OpenIddictLiteDBScopeStoreResolver(IServiceProvider provider)
        => _provider = provider ?? throw new ArgumentNullException(nameof(provider));

    /// <summary>
    /// Returns a scope store compatible with the specified scope type or throws an
    /// <see cref="InvalidOperationException"/> if no store can be built using the specified type.
    /// </summary>
    /// <typeparam name="TScope">The type of the Scope entity.</typeparam>
    /// <returns>An <see cref="IOpenIddictScopeStore{TScope}"/>.</returns>
    public IOpenIddictScopeStore<TScope> Get<TScope>() where TScope : class
    {
        var store = _provider.GetService<IOpenIddictScopeStore<TScope>>();
        if (store is not null)
        {
            return store;
        }

        var type = _cache.GetOrAdd(typeof(TScope), key =>
        {
            if (!typeof(OpenIddictLiteDBScope).IsAssignableFrom(key))
            {
                throw new InvalidOperationException("The specified scope type is not compatible with the LiteDB stores.\r\nWhen enabling the LiteDB stores, make sure you use the built-in 'OpenIddictLiteDBScope' entity or a custom entity that inherits from the 'OpenIddictLiteDBScope' entity.");
            }

            return typeof(OpenIddictLiteDBScopeStore<>).MakeGenericType(key);
        });

        return (IOpenIddictScopeStore<TScope>)_provider.GetRequiredService(type);
    }
}