// Load the recipe
#load nuget:?package=TestCentric.Cake.Recipe&version=1.1.2
// Comment out above line and uncomment below for local tests of recipe changes
//#load ../TestCentric.Cake.Recipe/recipe/*.cake

BuildSettings.Initialize
(
	context: Context,
	title: "Net40PluggableAgent",
	solutionFile: "net40-pluggable-agent.sln",
	unitTests: "**/*.tests.exe",
	githubRepository: "net40-pluggable-agent"
);

BuildSettings.Packages.AddRange(new PluggableAgentFactory(".NetFramework, Version=4.0").Packages);

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

Build.Run();
