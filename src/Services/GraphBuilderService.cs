using CodersTea.DeeplyDep.Models;
using CodersTea.DeeplyDep.Parsers;

namespace CodersTea.DeeplyDep.Services;

public class GraphBuilderService(SolutionParser solutionParser, ProjectParser projectParser)
{
    public DependencyGraph BuildGraph(string solutionPath)
    {
        var solutionRootNode = new Node
        (
            name: Path.GetFileName(solutionPath),
            fullPath: solutionPath,
            nodeType: NodeType.SOLUTION
        );

        var referencedProjects = solutionParser.ParaseAndGetDependentProjects(solutionPath);

        var dependencyGraph = new DependencyGraph(solutionRootNode);
        dependencyGraph.AllNodes[solutionRootNode.FullPath] = solutionRootNode;

        foreach (var dep in referencedProjects)
        {
            solutionRootNode.Dependencies.Add(BuildGraphRec(dep, dependencyGraph.AllNodes));
        }

        return dependencyGraph;
    }

    private Node BuildGraphRec(string projectPath, Dictionary<string, Node> allNodes)
    {
        // TODO: check for cycle?
        if (allNodes.TryGetValue(projectPath, out var graph))
        {
            return graph;
        }

        var node = new Node(Path.GetFileNameWithoutExtension(projectPath), projectPath);
        allNodes[projectPath] = node;

        foreach (var dependency in projectParser.ParseAndGetDependentProjects(projectPath))
        {
            var dependencyNode = BuildGraphRec(dependency, allNodes);
            node.Dependencies.Add(dependencyNode);
        }

        return node;
    }
}