using CommandLine;

namespace CodersTea.DeeplyDep.Models;

public record CliOptions
{
    
  [Value(0, Required = true, MetaName = "Full Solution Path", HelpText = ".Net Solution Path")]
  public required string SolutionPath { get; set; }
  
  [Option( 'v',  "verbose",  Required = false, HelpText = "Add Trace Logging")]
  public bool Verbose { get; set; }
}