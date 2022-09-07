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
/// Exposes a method allowing to resolve an authorization store.
/// </summary>
public class OpenIddictLiteDBAuthorizationStoreResolver : IOpenIddictAuthorizationStoreResolver
{
    private readonly ConcurrentDictionary<Type, Type> _cache = new ConcurrentDictionary<Type, Type>();
    private readonly IServiceProvider _provider;

    /// <summary>
    /// Creates a new instance of the <see cref="OpenIddictLiteDBAuthorizationStoreResolver"/> class.
    /// </summary>
    public OpenIddictLiteDBAuthorizationStoreResolver(IServiceProvider provider)
        => _provider = provider ?? throw new ArgumentNullException(nameof(provider));

    /// <summary>
    /// Returns an authorization store compatible with the specified authorization type or throws an
    /// <see cref="InvalidOperationException"/> if no store can be built using the specified type.
    /// </summary>
    /// <typeparam name="TAuthorization">The type of the Authorization entity.</typeparam>
    /// <returns>An <see cref="IOpenIddictAuthorizationStore{TAuthorization}"/>.</returns>
    public IOpenIddictAuthorizationStore<TAuthorization> Get<TAuthorization>() where TAuthorization : class
    {
        var store = _provider.GetService<IOpenIddictAuthorizationStore<TAuthorization>>();
        if (store is not null)
        {
            return store;
        }

        var type = _cache.GetOrAdd(typeof(TAuthorization), key =>
        {
            if (!typeof(OpenIddictLiteDBAuthorization).IsAssignableFrom(key))
            {
                throw new InvalidOperationException("The specified authorization type is not compatible with the LiteDB stores.\r\nWhen enabling the LiteDB stores, make sure you use the built-in 'OpenIddictLiteDBAuthorization' entity or a custom entity that inherits from the 'OpenIddictLiteDBAuthorization' entity.");
            }

            return typeof(OpenIddictLiteDBAuthorizationStore<>).MakeGenericType(key);
        });

        return (IOpenIddictAuthorizationStore<TAuthorization>)_provider.GetRequiredService(type);
    }
}