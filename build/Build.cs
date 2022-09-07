using System;
using System.Linq;
using Newtonsoft.Json;
using Nuke.Common;
using Nuke.Common.CI;
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("GitHub access token used for creating a new or updating an existing release.")]
    readonly string GitHubAccessToken;

    [Parameter("NuGet source used for pushing the Sdk NuGet package. Default is NuGet.org.")]
    readonly string NuGetSource = "https://api.nuget.org/v3/index.json";

    [Parameter("NuGet API key used to pushing the Sdk NuGet package.")]
    readonly string NuGetApiKey;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion(Framework = "net6.0", NoFetch = true)] readonly GitVersion GitVersion;

    static AbsolutePath SourceDirectory => RootDirectory / "src";
    static AbsolutePath TestsDirectory => RootDirectory / "test";
    static AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    const string GitHubRepositoryName = "InRuleGitStorage";
    const string GitHubRepositoryOwner = "openiddict-litedb";

    readonly string[] NuGetRestoreSources = new[] {
        "https://api.nuget.org/v3/index.json"
    };

    protected override void OnBuildInitialized()
    {
        Log.Information($"GitVersion settings:\n{JsonConvert.SerializeObject(GitVersion, Formatting.Indented)}");
    }

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            Log.Debug("Deleting all bin/obj directories...");
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);

            Log.Debug("Cleaning artifacts directory...");
            EnsureCleanDirectory(ArtifactsDirectory);

            Log.Debug("Deleting test results directories...");
            TestsDirectory.GlobDirectories("**/TestResults").ForEach(DeleteDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            Log.Debug("Restoring NuGet packages for solution...");
            DotNetRestore(s => s
                .SetProjectFile(Solution)
                .SetSources(NuGetRestoreSources));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            Log.Debug("Compiling solution...");
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetVersion(GitVersion.FullSemVer)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .SetProperty("RepositoryBranch", GitVersion.BranchName)
                .SetProperty("RepositoryCommit", GitVersion.Sha)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });

}
