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

    Target PublishToGitHub => _ => _
        .DependsOn(PublishArtifacts)
        .Requires(() => GitHubAccessToken)
        .Executes(async () =>
        {
            Log.Debug($"Creating 'v{GitVersion.SemVer}' release in GitHub...");
            var github = new Octokit.GitHubClient(new Octokit.ProductHeaderValue("sknet.openiddict.litedb.build"))
            {
                Credentials = new Octokit.Credentials(GitHubAccessToken)
            };

            Octokit.Release release = null;
            try
            {
                Log.Information($"Retrieving existing release tagged as 'v{GitVersion.SemVer}'...");
                release = await github.Repository.Release.Get(GitHubRepositoryOwner, GitHubRepositoryName, $"v{GitVersion.SemVer}");
            }
            catch (Octokit.NotFoundException)
            {
                Log.Information("Release not found. Retrieving existing draft release...");
                var releases = await github.Repository.Release.GetAll(GitHubRepositoryOwner, GitHubRepositoryName);
                release = releases.SingleOrDefault(r => r.Draft && (r.TagName == $"v{GitVersion.SemVer}" || r.TagName.StartsWith($"v{GitVersion.MajorMinorPatch}-{GitVersion.PreReleaseLabel}")));
            }

            if (release != null)
            {
                Log.Information($"Release '{release.Name}' found. Updating release...");
                release = await github.Repository.Release.Edit(GitHubRepositoryOwner, GitHubRepositoryName, release.Id, new Octokit.ReleaseUpdate
                {
                    Name = $"v{GitVersion.SemVer}",
                    TagName = $"v{GitVersion.SemVer}",
                    Body = !string.IsNullOrWhiteSpace(release.Body)
                        ? release.Body
                        : $"Release notes for `v{GitVersion.SemVer}` are not available at this time.",
                    Draft = release.Draft,
                    Prerelease = !string.IsNullOrWhiteSpace(GitVersion.PreReleaseTag),
                    TargetCommitish = GitRepository.Commit
                });
            }
            else
            {
                Log.Information($"Release not found. Creating a new draft release...");
                release = await github.Repository.Release.Create(GitHubRepositoryOwner, GitHubRepositoryName, new Octokit.NewRelease($"v{GitVersion.SemVer}")
                {
                    Name = $"v{GitVersion.SemVer}",
                    Body = $"Release notes for `v{GitVersion.SemVer}` are not available at this time.",
                    Draft = true,
                    Prerelease = !string.IsNullOrWhiteSpace(GitVersion.PreReleaseTag),
                    TargetCommitish = GitRepository.Commit
                });
            }

            Log.Information("Removing existing assets (if any)...");
            var assets = await github.Repository.Release.GetAllAssets(GitHubRepositoryOwner, GitHubRepositoryName, release.Id);
            foreach (var asset in assets)
            {
                await github.Repository.Release.DeleteAsset(GitHubRepositoryOwner, GitHubRepositoryName, asset.Id);
            }

            var artifacts = ArtifactsDirectory.GlobFiles($"Sknet.*.{GitVersion.SemVer}.*");
            foreach (var artifact in artifacts)
            {
                var file = new FileInfo(artifact);
                using var stream = File.OpenRead(artifact);

                Log.Information($"Uploading asset '{file.Name}'...");
                var asset = await github.Repository.Release.UploadAsset(release, new Octokit.ReleaseAssetUpload()
                {
                    ContentType = "application/zip",
                    FileName = file.Name,
                    RawData = stream,
                });
            }
        });
}
