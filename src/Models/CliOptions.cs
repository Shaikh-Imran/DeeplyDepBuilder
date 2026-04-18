using CommandLine;

namespace CodersTea.DeeplyDep.Models;

public record CliOptions
{
    [Option('p', "project", Required = true, HelpText = ".Net Project or Solution Full Path")]
    public required string ProjectOrSolutionPath { get; set; }

    [Option('v', "verbose", Required = false, HelpText = "Add Trace Logging")]
    public bool Verbose { get; set; }
    
    [Option('g', "graph-visualize",  Required = false, HelpText = "Create Diagraph file for graph visualization" )]
    public string VisualizeGraph { get; set; }
   
    [Option("show-path", Required = false, Default = false, HelpText = "If True shows path otherwise File name. Names can have duplicates")]
    public bool ShowPathInGraph { get; set; }
}