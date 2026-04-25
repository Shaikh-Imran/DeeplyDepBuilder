using System.Text;
using CodersTea.DeeplyDep.Models;
using CodersTea.DeeplyDep.Parsers;
using CodersTea.DeeplyDep.Services;
using CodersTea.DeeplyDep.Utils;
using CommandLine;

namespace CodersTea.DeeplyDep;

public class Program
{
    public static async Task Main(string[] args)
    {
        var opts = Parser.Default.ParseArguments<CliOptions>(args).Value;
        if (opts.Verbose)
        {
            Logger.isTraceEnabled = true;
        }

        PathUtil.CurrentPlatform = Platform.Linux; // TODO: get automatic platform

        Logger.Info("Starting CodersTea.DeeplyDep CLI");

        var projectOrSolutionPath = opts.ProjectOrSolutionPath.Trim();

        if (!Path.Exists(projectOrSolutionPath))
        {
            Logger.Error($"Given Solution or Project path does not exist.:  {opts.ProjectOrSolutionPath}");
            Logger.Error("Exiting...");
            throw new ArgumentException(
                $"Given Solution or Project path does not exist.:  {opts.ProjectOrSolutionPath}");
        }

        var graphBuilder = new GraphBuilderService(new SolutionParser(), new ProjectParser());
        var depencyGraph = graphBuilder.BuildGraph(projectOrSolutionPath);
        var topoSort = new TopoSortingService().TopoSortWithLevels(depencyGraph);

        if (!string.IsNullOrEmpty(opts.VisualizeGraph))
        {
            // PrintGraph(depencyGraph, opts);
            PrintGraphMermaid(depencyGraph, topoSort, opts);
        }

        await new BuildExecutor().BuildProjects(topoSort, opts);
    }

    // diagraph priting
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

    private static void PrintGraphMermaid(DependencyGraph dependencyGraph, List<List<Node>> topoSorted, CliOptions opts)
    {
        Logger.Info($"Building Visual Graph of Mermaid in {opts.VisualizeGraph}");

        var mermaidNodeDict = dependencyGraph.AllNodes.Values.Select((key, index) => new { key, index }).ToDictionary(
            x => GetValue(x.key),
            x => "p" + x.index
        );

        var nodeNames = string.Join('\n', mermaidNodeDict.Select(kv => $"\t{kv.Value}[\"{kv.Key}\"]"));

        // Actual Dependency Graph
        var dependencyGraphString = String.Join('\n',
            dependencyGraph.AllNodes.Values.Select(node => node.Dependencies.Select(d =>
                    $"\t{mermaidNodeDict[GetValue(node)]} --> {mermaidNodeDict[GetValue(d)]}"))
                .SelectMany(x => x)
                .ToList()
        );

        // TOpo sort graph
        var sb = new List<string>();
        for (var i = 0; i < topoSorted.Count; i++)
        {
            sb.Add($"\tsubgraph LEVEL{i}");
            sb.Add($"\t\tdirection TD");
            foreach (var node in topoSorted[i])
            {
                sb.Add($"\t\t{mermaidNodeDict[GetValue(node)]}");
            }

            sb.Add($"\tend");
        }

        for (var i = 1; i < topoSorted.Count; i++)
        {
            sb.Add($"\tLEVEL{i - 1} --> LEVEL{i}");
        }

        var topoSortedMermaidGraph = string.Join('\n', sb);

        var fileData = $"""
                        ## Dependency Graph

                        This shows the dependency from the given path upto leaf project.

                        ```mermaid
                        flowchart TD
                        {nodeNames}
                        {dependencyGraphString}
                        ```

                        ## Topological Sort with Levels
                        This shws the topological sort. Each level can be build parallelly to reduce build time.
                        When building, we start from the bottom i.e. the last level (leaf nodes).

                        ```mermaid
                        flowchart TD
                        {nodeNames}
                        {topoSortedMermaidGraph}
                        ```
                        """;

        File.WriteAllText(opts.VisualizeGraph, fileData);
        Logger.Info($"Diagram file created at {opts.VisualizeGraph}");

        return;

        string GetValue(Node node) => opts.ShowPathInGraph ? node.FullPath : node.Name;
    }
}