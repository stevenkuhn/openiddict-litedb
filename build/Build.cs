[ShutdownDotNetAfterServerBuild]
class Build : NukeBuild
{
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
        .DependsOn(Clean)
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

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            Log.Debug("Running tests for solution...");
            DotNetTest(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoBuild()
                .EnableNoRestore());
        });

    Target PublishArtifacts => _ => _
        .DependsOn(Compile)
        .After(Test)
        .Executes(() =>
        {
            Log.Debug("Publishing artifacts to the artifacts folder...");
            SourceDirectory
                .GlobFiles($"**/{Configuration}/**/Sknet.*.{GitVersion.SemVer}.*nupkg")
                .ForEach(file => CopyFileToDirectory(file, ArtifactsDirectory));
        });
}
