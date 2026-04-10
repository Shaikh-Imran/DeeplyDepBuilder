using System.Text.RegularExpressions;
using CodersTea.DeeplyDep.Models;
using CodersTea.DeeplyDep.Utils;

namespace CodersTea.DeeplyDep.Parsers;

public class SolutionParser
{
    
    public Solution ParseSolution(string solutionPath)
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

        var solution = new Solution()
        {
            SolutionRoot = solutionNode,
        };
        
        var projectMatches = Regex.Matches(content,
            @"Project\(""\{[^}]+\}""\)\s*=\s*""([^""]+)"",\s*""([^""]+)""");


        foreach (var projectMatchGroup in projectMatches.OfType<Match>())
        {
            var projectPath = projectMatchGroup.Groups[2].Value.Trim();

            if (!projectPath.EndsWith(".csproj", StringComparison.InvariantCulture)
                && !projectPath.EndsWith(".fsproj", StringComparison.InvariantCulture)
               )
            {
                Logger.Error($"Could not find project file {projectPath}");
                continue;
            }

            var fullPath = PlatformUtil.ToPlatformPath(Path.GetFullPath(Path.Combine(solutionDir ?? "", projectPath)));
            
            if(!File.Exists(fullPath))
            {
                Logger.Error($"Could not find project file {fullPath}");
                continue;
            }

            var projectNode = new Node()
            {
                Name = projectMatchGroup.Groups[1].Value,
                FullPath = fullPath,
                NodeType = NodeType.PROJECT
            };
           
            solutionNode.Dependencies.Add(projectNode);
        }
        
        return solution;
    }
}