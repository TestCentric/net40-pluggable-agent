#tool nuget:?package=GitVersion.CommandLine&version=5.0.0

#load nuget:?package=TestCentric.Cake.Recipe&version=1.0.0-dev00025

var target = Argument("target", "Default");

//////////////////////////////////////////////////////////////////////
// SETUP
//////////////////////////////////////////////////////////////////////

Setup<BuildSettings>((context) =>
{
	var settings = BuildSettings.Initialize
	(
		context: context,
		title: "Net40PluggableAgent",
		solutionFile: "net40-pluggable-agent.sln",
		unitTest: "net40-agent-launcher.tests.exe",
		guiVersion: "2.0.0-dev00081",
		githubOwner: "TestCentric",
		githubRepository: "net40-pluggable-agent",
		copyright: "Copyright (c) Charlie Poole and TestCentric Engine contributors."
	);

	settings.Packages.Add
	(
		new NuGetPackage
		(
			settings,
			"NUnit.Extension.Net40PluggableAgent",
			"nuget/Net40PluggableAgent.nuspec"
		)
		{
			PackageChecks = new PackageCheck[]
			{
				HasFiles("LICENSE.txt", "CHANGES.txt"),
				HasDirectory("tools").WithFiles(MY_LAUNCHER_FILES),
				HasDirectory("tools/agent").WithFiles(MY_AGENT_FILES)
			}
		}
	);

	settings.Packages.Add
	(
		new ChocolateyPackage
		(
			settings,
			"nunit-extension-net40-pluggable-agent",
			"choco/net40-pluggable-agent.nuspec"
		)
		{
			PackageChecks = new PackageCheck[]
			{
				HasDirectory("tools").WithFiles(MY_LAUNCHER_FILES)
					.WithFiles("LICENSE.txt", "CHANGES.txt", "VERIFICATION.txt"),
				HasDirectory("tools/agent").WithFiles(MY_AGENT_FILES)
			}
		}
	);

	Information($"Net40PluggableAgent {settings.Configuration} version {settings.PackageVersion}");

	if (BuildSystem.IsRunningOnAppVeyor)
		AppVeyor.UpdateBuildVersion(settings.PackageVersion);

	return settings;
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Appveyor")
	.IsDependentOn("Build")
	.IsDependentOn("Test")
	.IsDependentOn("Package")
	.IsDependentOn("Publish");

Task("Full")
	.IsDependentOn("Build")
	.IsDependentOn("Test")
	.IsDependentOn("Package");

//Task("Travis")
//	.IsDependentOn("Build")
//	.IsDependentOn("Test");

Task("Default")
    .IsDependentOn("Build");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
