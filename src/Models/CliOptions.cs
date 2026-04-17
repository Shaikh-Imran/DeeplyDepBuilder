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
}