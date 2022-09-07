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
/// Exposes a method allowing to resolve an application store.
/// </summary>
public class OpenIddictLiteDBApplicationStoreResolver : IOpenIddictApplicationStoreResolver
{
    private readonly ConcurrentDictionary<Type, Type> _cache = new ConcurrentDictionary<Type, Type>();
    private readonly IServiceProvider _provider;

    public OpenIddictLiteDBApplicationStoreResolver(IServiceProvider provider)
        => _provider = provider ?? throw new ArgumentNullException(nameof(provider));

    /// <summary>
    /// Returns an application store compatible with the specified application type or throws an
    /// <see cref="InvalidOperationException"/> if no store can be built using the specified type.
    /// </summary>
    /// <typeparam name="TApplication">The type of the Application entity.</typeparam>
    /// <returns>An <see cref="IOpenIddictApplicationStore{TApplication}"/>.</returns>
    public IOpenIddictApplicationStore<TApplication> Get<TApplication>() where TApplication : class
    {
        var store = _provider.GetService<IOpenIddictApplicationStore<TApplication>>();
        if (store is not null)
        {
            return store;
        }

        var type = _cache.GetOrAdd(typeof(TApplication), key =>
        {
            if (!typeof(OpenIddictLiteDBApplication).IsAssignableFrom(key))
            {
                throw new InvalidOperationException("The specified application type is not compatible with the LiteDB stores.\r\nWhen enabling the LiteDB stores, make sure you use the built-in 'OpenIddictLiteDBApplication' entity or a custom entity that inherits from the 'OpenIddictLiteDBApplication' entity.");
            }

            return typeof(OpenIddictLiteDBApplicationStore<>).MakeGenericType(key);
        });

        return (IOpenIddictApplicationStore<TApplication>)_provider.GetRequiredService(type);
    }
}