using CommandLine;

namespace CodersTea.DeeplyDep.Models;

public record CliOptions
{
    [Option('s', "solution", Required = true, HelpText = ".Net Solution Full Path")]
    public required string SolutionPath { get; set; }

    [Option('v', "verbose", Required = false, HelpText = "Add Trace Logging")]
    public bool Verbose { get; set; }
    
    [Option('g', "graph-visualize",  Required = false, HelpText = "Create Diagraph file for graph visualization" )]
    public string VisualizeGraph { get; set; }
   
    [Option('p', "show-path", Required = false, Default = false, HelpText = "If True shows path otherwise File name. Names can have duplicates")]
    public bool ShowPathInGraph { get; set; }
}