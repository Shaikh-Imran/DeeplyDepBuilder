namespace CodersTea.DeeplyDep.Models;

public class Node
{
    public string Name { get; set; } = string.Empty;
    public string FullPath { get; set; } = string.Empty;
    public NodeType NodeType { get; set; }
    public List<Node> Dependencies { get; set; } = new();
    public int Depth { get; set; }

    public override string ToString()
    {
        return $"Name: {Name}, FullPath: {FullPath}, NodeType: {NodeType}";
    }
}