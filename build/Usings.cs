﻿global using System;
global using System.ComponentModel;
global using System.IO;
global using System.Linq;

global using Newtonsoft.Json;
global using Serilog;
global using Nuke.Common;
global using Nuke.Common.CI;
global using Nuke.Common.Execution;
global using Nuke.Common.Git;
global using Nuke.Common.IO;
global using Nuke.Common.ProjectModel;
global using Nuke.Common.Tooling;
global using Nuke.Common.Tools.DotNet;
global using Nuke.Common.Tools.GitVersion;
global using Nuke.Common.Tools.MSBuild;
global using Nuke.Common.Tools.NuGet;
global using Nuke.Common.Utilities.Collections;

global using static Nuke.Common.EnvironmentInfo;
global using static Nuke.Common.IO.CompressionTasks;
global using static Nuke.Common.IO.FileSystemTasks;
global using static Nuke.Common.IO.PathConstruction;
global using static Nuke.Common.Tools.DotNet.DotNetTasks;
global using static Nuke.Common.Tools.NuGet.NuGetTasks;