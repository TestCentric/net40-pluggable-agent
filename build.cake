#tool nuget:?package=GitVersion.CommandLine&version=5.0.0
#tool nuget:?package=GitReleaseManager&version=0.11.0
#tool nuget:?package=TestCentric.GuiRunner&version=2.0.0-dev00089

#load nuget:?package=TestCentric.Cake.Recipe&version=1.0.0-dev00030

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
		guiVersion: "2.0.0-dev00089",
		githubOwner: "TestCentric",
		githubRepository: "net40-pluggable-agent",
		copyright: "Copyright (c) Charlie Poole and TestCentric Engine contributors.",
		packages: new PackageDefinition[] { NuGetAgentPackage, ChocolateyAgentPackage },
		packageTests: new PackageTest[] { Net20PackageTest, Net35PackageTest, Net45PackageTest }
	);

	Information($"Net40PluggableAgent {settings.Configuration} version {settings.PackageVersion}");

	if (BuildSystem.IsRunningOnAppVeyor)
		AppVeyor.UpdateBuildVersion(settings.PackageVersion);

	return settings;
});

var NuGetAgentPackage = new NuGetPackage(
		id: "NUnit.Extension.Net40PluggableAgent",
		source: "nuget/Net40PluggableAgent.nuspec",
		checks: new PackageCheck[] {
			HasFiles("LICENSE.txt", "CHANGES.txt"),
			HasDirectory("tools").WithFiles("net40-agent-launcher.dll", "nunit.engine.api.dll"),
			HasDirectory("tools/agent").WithFiles(
				"net40-pluggable-agent.exe", "net40-pluggable-agent.exe.config",
				"net40-pluggable-agent-x86.exe", "net40-pluggable-agent-x86.exe.config",
				"nunit.engine.api.dll", "testcentric.engine.core.dll")
		});

var ChocolateyAgentPackage = new ChocolateyPackage(
		id: "nunit-extension-net40-pluggable-agent",
		source: "choco/net40-pluggable-agent.nuspec",
		checks: new PackageCheck[] {
			HasDirectory("tools").WithFiles("net40-agent-launcher.dll", "nunit.engine.api.dll")
				.WithFiles("LICENSE.txt", "CHANGES.txt", "VERIFICATION.txt"),
			HasDirectory("tools/agent").WithFiles(
				"net40-pluggable-agent.exe", "net40-pluggable-agent.exe.config",
				"net40-pluggable-agent-x86.exe", "net40-pluggable-agent-x86.exe.config",
				"nunit.engine.api.dll", "testcentric.engine.core.dll")
		});

var Net20PackageTest = new PackageTest(
	1, "Run mock-assembly.dll targeting .NET 2.0", GUI_RUNNER,
	"tests/net20/mock-assembly.dll", CommonResult);

var Net35PackageTest = new PackageTest(
	1, "Run mock-assembly.dll targeting .NET 3.5", GUI_RUNNER,
	"tests/net35/mock-assembly.dll", CommonResult);

var Net45PackageTest = new PackageTest(
	1, "Run mock-assembly.dll targeting .NET 4.5", GUI_RUNNER,
	"tests/net45/mock-assembly.dll", CommonResult);

static readonly string GUI_RUNNER = "tools/TestCentric.GuiRunner.2.0.0-dev00089/tools/testcentric.exe";

ExpectedResult CommonResult => new ExpectedResult("Failed")
{
	Total = 36,
	Passed = 23,
	Failed = 5,
	Warnings = 1,
	Inconclusive = 1,
	Skipped = 7,
	Assemblies = new ExpectedAssemblyResult[]
	{
		new ExpectedAssemblyResult("mock-assembly.dll", "Net40AgentLauncher")
	}
};

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
