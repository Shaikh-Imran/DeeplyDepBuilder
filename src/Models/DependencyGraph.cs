namespace CodersTea.DeeplyDep.Models;

public class DependencyGraph
{
    public Node RootNode { get; init; }
    public Dictionary<string, Node> AllNodes { get; init; }

    public DependencyGraph(Node rootNode)
    {
        RootNode = rootNode;
        AllNodes = new Dictionary<string, Node>();
    }
}