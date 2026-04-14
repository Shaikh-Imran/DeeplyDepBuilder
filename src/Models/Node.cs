namespace CodersTea.DeeplyDep.Models;

public class Node
{
    public string Name { get; init; }
    public string FullPath { get; init; }
    public NodeType NodeType { get; init; }
    public List<Node> Dependencies { get; init; }

    public Node(string name, string fullPath, NodeType nodeType = NodeType.PROJECT)
    {
        Name = name;
        FullPath = fullPath;
        NodeType = nodeType;
        Dependencies = new();
    }

    public override string ToString()
    {
        return $"Name: {Name}, FullPath: {FullPath}, NodeType: {NodeType}";
    }
}