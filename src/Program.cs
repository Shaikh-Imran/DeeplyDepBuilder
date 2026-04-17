using CodersTea.DeeplyDep.Models;
using CodersTea.DeeplyDep.Parsers;
using CodersTea.DeeplyDep.Services;
using CodersTea.DeeplyDep.Utils;
using CommandLine;

namespace CodersTea.DeeplyDep;

public class Program
{
    public static void Main(string[] args)
    {
        var opts = Parser.Default.ParseArguments<CliOptions>(args).Value;
        if (opts.Verbose)
        {
            Logger.isTraceEnabled = true;
        }

        PlatformUtil.CurrentPlatform = Platform.Linux;

        Logger.Info("Starting CodersTea.DeeplyDep CLI");

        var solutionPath = opts.SolutionPath.Trim();

        if (!solutionPath.EndsWith(".sln"))
        {
            Logger.Error("Given path is not a solution file. Please provide a valid .sln file path.");
            throw new ArgumentException("Not a solution file");
        }

        if (!Path.Exists(solutionPath))
        {
            Logger.Error($"Given Solution path does not exist.:  {opts.SolutionPath}");
            Logger.Error("Exiting...");
            throw new ArgumentException($"Given Solution path does not exist.:  {opts.SolutionPath}");
        }

        var graphBuilder = new GraphBuilderService(new SolutionParser(), new ProjectParser());
        var depencyGraph = graphBuilder.BuildGraph(solutionPath);

        if (!string.IsNullOrEmpty(opts.VisualizeGraph))
            PrintGraph(depencyGraph, opts);
    }

    private static void PrintGraph(DependencyGraph dependencyGraph, CliOptions opts)
    {
        Logger.Info($"Building Visual Graph of diagraph in {opts.VisualizeGraph}");
        var graph = String.Join('\n',
            dependencyGraph.AllNodes.Values.Select(node => node.Dependencies.Select(d =>
                    $"\t '{node.Name}' -> '{d.Name}'".Replace("'", "\"")))
                .SelectMany(x => x)
                .ToList()
        );

        var dependencyGraphString = $" digraph G {{ \n {graph} \n}}";

        File.WriteAllText(
            opts.VisualizeGraph,
            dependencyGraphString);
        Logger.Info($"Diagraph file created at {opts.VisualizeGraph}");
    }
}