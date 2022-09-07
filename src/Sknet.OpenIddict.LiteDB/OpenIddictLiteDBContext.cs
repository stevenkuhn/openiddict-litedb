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

/// <inheritdoc/>
public class OpenIddictLiteDBContext : IOpenIddictLiteDBContext
{
    private readonly IOptionsMonitor<OpenIddictLiteDBOptions> _options;
    private readonly IServiceProvider _provider;

    public OpenIddictLiteDBContext(
        IOptionsMonitor<OpenIddictLiteDBOptions> options,
        IServiceProvider provider)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    /// <inheritdoc/>
    public ValueTask<ILiteDatabase> GetDatabaseAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return new(Task.FromCanceled<ILiteDatabase>(cancellationToken));
        }

        var database = _options.CurrentValue.Database;
        if (database is null)
        {
            database = _provider.GetService<ILiteDatabase>();
        }

        if (database is null)
        {
            return new(Task.FromException<ILiteDatabase>(
                new InvalidOperationException("No suitable LiteDB database service can be found.\r\nTo configure the OpenIddict LiteDB stores to use a specific database, use 'services.AddOpenIddict().AddCore().UseLiteDB().UseDatabase()' or register an 'ILiteDatabase' in the dependency injection container in 'ConfigureServices()'.")));
        }

        return new(database);
    }
}