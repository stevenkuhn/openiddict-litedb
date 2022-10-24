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

static class ILiteDatabaseExtensions
{
    public static ILiteCollection<OpenIddictLiteDBApplication> Applications(this ILiteDatabase database)
    {
        var options = new OpenIddictLiteDBOptions();

        return database.GetCollection<OpenIddictLiteDBApplication>(options.ApplicationsCollectionName);
    }

    public static ILiteCollection<OpenIddictLiteDBAuthorization> Authorizations(this ILiteDatabase database)
    {
        var options = new OpenIddictLiteDBOptions();

        return database.GetCollection<OpenIddictLiteDBAuthorization>(options.AuthorizationsCollectionName);
    }

    public static ILiteCollection<OpenIddictLiteDBScope> Scopes(this ILiteDatabase database)
    {
        var options = new OpenIddictLiteDBOptions();

        return database.GetCollection<OpenIddictLiteDBScope>(options.ScopesCollectionName);
    }
    
    public static ILiteCollection<OpenIddictLiteDBToken> Tokens(this ILiteDatabase database)
    {
        var options = new OpenIddictLiteDBOptions();

        return database.GetCollection<OpenIddictLiteDBToken>(options.TokensCollectionName);
    }
}