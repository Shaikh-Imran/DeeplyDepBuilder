using CodersTea.DeeplyDep.Models;
using CodersTea.DeeplyDep.Parsers;

namespace CodersTea.DeeplyDep.Services;

public class GraphBuilderService(SolutionParser solutionParser, ProjectParser projectParser)
{
    public DependencyGraph BuildGraph(string solutionPath)
    {
        var allNodes = new Dictionary<string, Node>();
        var rootNode = BuildGraphRec(solutionPath, allNodes);
        return new DependencyGraph(rootNode, allNodes);
    }

    private Node BuildGraphRec(string projectPath, Dictionary<string, Node> allNodes)
    {
        // TODO: check for cycle?
        if (allNodes.TryGetValue(projectPath, out var graph))
        {
            return graph;
        }

        var projectType = projectPath.EndsWith(".sln") ? NodeType.SOLUTION : NodeType.PROJECT;

        var node = new Node(Path.GetFileNameWithoutExtension(projectPath), projectPath, projectType);
        allNodes[projectPath] = node;

        var dependecies = projectType == NodeType.SOLUTION
            ? solutionParser.ParaseAndGetDependentProjects(projectPath)
            : projectParser.ParseAndGetDependentProjects(projectPath);

        foreach (var dependency in dependecies)
        {
            var dependencyNode = BuildGraphRec(dependency, allNodes);
            node.Dependencies.Add(dependencyNode);
        }

        return node;
    }
}