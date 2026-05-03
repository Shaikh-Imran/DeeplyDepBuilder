using CommandLine;

namespace CodersTea.DeeplyDep.Models;

public record CliOptions
{
    [Option('p', "project", Required = true, HelpText = ".Net Project or Solution Full Path")]
    public required string ProjectOrSolutionPath { get; set; }

    [Option('v', "verbose", Required = false, HelpText = "Add Trace Logging")]
    public bool Verbose { get; set; }

    [Option('c', "clean", Required = false, Default = false, HelpText = "Do Dotnet Clean instead of Dotnet Build")]
    public bool Clean { get; set; }

    [Option('g', "generate-graph-path", Required = false,
        HelpText = "Generate markdown file for graph visualization in given path")]
    public string? VisualizeGraphPath { get; set; }

    [Option("hide-path-in-graph", Required = false, Default = false,
        HelpText = "In visual graph, if false shows path otherwise File name. Use only when names are unique")]
    public bool HidePathInGraph { get; set; }

    [Option("no-parallel", Required = false, Default = false,
        HelpText = "If false builds projects in the same level in parallelly otherwise sequentially")]
    public bool NoParallelBuild { get; set; }

    [Option("show-build-output", Required = false, Default = false,
        HelpText = "If True shows the build output in the console")]
    public bool ShowBuildOutput { get; set; }
}