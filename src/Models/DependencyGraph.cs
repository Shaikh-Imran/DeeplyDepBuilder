namespace CodersTea.DeeplyDep.Models;

public class DependencyGraph(Node rootNode, Dictionary<string, Node> allNodes)
{
    public Node RootNode { get; init; } = rootNode;
    public Dictionary<string, Node> AllNodes { get; init; } = allNodes;
}