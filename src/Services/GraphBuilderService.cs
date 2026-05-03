using CodersTea.DeeplyDep.Models;
using CodersTea.DeeplyDep.Parsers;
using CodersTea.DeeplyDep.Utils;

namespace CodersTea.DeeplyDep.Services;

public class GraphBuilderService(SolutionParser solutionParser, ProjectParser projectParser)
{
    public DependencyGraph BuildGraph(string solutionPath)
    {
        var allNodes = new Dictionary<string, Node>();
        var rootNode = BuildGraphRec(solutionPath, allNodes, new HashSet<string>());
        return new DependencyGraph(rootNode, allNodes);
    }

    private Node BuildGraphRec(string projectPath, Dictionary<string, Node> allNodes, HashSet<string> visitedPath)
    {
        if (visitedPath.Contains(projectPath))
        {
            Logger.Error($"Cyclic Dependency Found. The Project is already referenced earlier : {projectPath}");
            throw new Exception($"Found Cyclic Dependency in the graph at path: {projectPath}");
        }
        
        if (allNodes.TryGetValue(projectPath, out var graph))
        {
            return graph;
        }
        
        visitedPath.Add(projectPath);

        var projectType = projectPath.EndsWith(".sln") ? NodeType.SOLUTION : NodeType.PROJECT;

        var node = new Node(Path.GetFileNameWithoutExtension(projectPath), projectPath, projectType);
        allNodes[projectPath] = node;

        var dependecies = projectType == NodeType.SOLUTION
            ? solutionParser.ParaseAndGetDependentProjects(projectPath)
            : projectParser.ParseAndGetDependentProjects(projectPath);

        foreach (var dependencyPath in dependecies)
        {
            Logger.Trace($"Processing dependency: {dependencyPath} for project: {projectPath}");
            var dependencyNode = BuildGraphRec(dependencyPath, allNodes, visitedPath);
            node.Dependencies.Add(dependencyNode);
        }
        
        visitedPath.Remove(projectPath);
        
        return node;
    }
}