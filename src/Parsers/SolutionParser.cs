using System.Text.RegularExpressions;
using CodersTea.DeeplyDep.Models;
using CodersTea.DeeplyDep.Utils;

namespace CodersTea.DeeplyDep.Parsers;

public class SolutionParser
{
    public Node ParseSolution(string solutionPath)
    {
        Logger.Info($"Parsing Solution {solutionPath}");

        var content = File.ReadAllText(solutionPath);

        var solutionDir = Path.GetDirectoryName(solutionPath);

        var solutionNode = new Node()
        {
            Name = Path.GetFileName(solutionPath),
            FullPath = solutionPath,
            NodeType = NodeType.SOLUTION,
            Dependencies = new List<Node>()
        };

        ParseAndLoadDependentProjects(content, solutionDir, solutionNode);

        Logger.Info($"Solution parsed with {solutionNode.Dependencies.Count} projects");


        return solutionNode;
    }

    private static void ParseAndLoadDependentProjects(string content, string? solutionDir, Node solutionNode)
    {
        var projectMatches = Regex.Matches(content,
            @"Project\(""\{[^}]+\}""\)\s*=\s*""([^""]+)"",\s*""([^""]+)""");
        
        Logger.Trace($"Found {projectMatches.Count} project references in solution file");

        foreach (var projectMatchGroup in projectMatches.OfType<Match>())
        {
            var projectPath = projectMatchGroup.Groups[2].Value.Trim();

            if (!projectPath.EndsWith(".csproj", StringComparison.InvariantCulture)
                && !projectPath.EndsWith(".fsproj", StringComparison.InvariantCulture)
               )
            {
                Logger.Trace($"Skipping non-project reference {projectPath}");
                continue;
            }

            var fullPath = PlatformUtil.ToPlatformPath(Path.GetFullPath(Path.Combine(solutionDir ?? "", projectPath)));

            if (!File.Exists(fullPath))
            {
                // TODO: need better handling, may be error log and exit. nio point in building graph if it going to fail anyway.
                Logger.Error($"Could not find project file {fullPath}. Conitinuing with next project reference.");
                continue;
            }

            var projectNode = new Node()
            {
                Name = projectMatchGroup.Groups[1].Value,
                FullPath = fullPath,
                NodeType = NodeType.PROJECT
            };

            solutionNode.Dependencies.Add(projectNode);
            Logger.Trace(
                $"Added project {projectNode.Name} with path {projectNode.FullPath} to solution {solutionNode.Name}");
        }
    }
}