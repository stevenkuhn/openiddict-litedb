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
/// Provides various settings needed to configure the OpenIddict LiteDB integration.
/// </summary>
public class OpenIddictLiteDBOptions
{
    /// <summary>
    /// Gets or sets the name of the applications collection (by default, openiddict_applications).
    /// </summary>
    public string ApplicationsCollectionName { get; set; } = "openiddict_applications";

    /// <summary>
    /// Gets or sets the name of the authorizations collection (by default, openiddict_authorizations).
    /// </summary>
    public string AuthorizationsCollectionName { get; set; } = "openiddict_authorizations";

    /// <summary>
    /// Gets or sets the <see cref="ILiteDatabase"/> used by the OpenIddict stores.
    /// If no value is explicitly set, the database is resolved from the DI container.
    /// </summary>
    public ILiteDatabase? Database { get; set; }

    /// <summary>
    /// Gets or sets the name of the scopes collection (by default, openiddict_scopes).
    /// </summary>
    public string ScopesCollectionName { get; set; } = "openiddict_scopes";

    /// <summary>
    /// Gets or sets the name of the tokens collection (by default, openiddict_tokens).
    /// </summary>
    public string TokensCollectionName { get; set; } = "openiddict_tokens";
}