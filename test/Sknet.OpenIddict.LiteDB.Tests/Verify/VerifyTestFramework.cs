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
[assembly: TestFramework("Sknet.OpenIddict.LiteDB.Tests.VerifyTestFramework", "Sknet.OpenIddict.LiteDB.Tests")]

namespace Sknet.OpenIddict.LiteDB.Tests;

public class VerifyTestFramework : XunitTestFramework
{
    public VerifyTestFramework(IMessageSink messageSink)
        : base(messageSink)
    {
        VerifierSettings.DerivePathInfo((sourceFile, projectDirectory, type, method) =>
        {
            var sourceFilePath = Path.GetDirectoryName(sourceFile)!;
            var sourceFileRelativePath = sourceFilePath.Substring(projectDirectory.Length);

            var directory = Path.Combine(
                NCrunchEnvironment.NCrunchIsResident()
                    ? Path.GetDirectoryName(NCrunchEnvironment.GetOriginalProjectPath())!
                    : projectDirectory!,
                "Snapshots",
                sourceFileRelativePath
            );

            return new(
                directory: directory,
                typeName: type.Name,
                methodName: method.Name);
        });

        VerifySystemJson.Enable();
    }
}
