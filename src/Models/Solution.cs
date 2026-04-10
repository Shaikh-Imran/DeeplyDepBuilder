namespace CodersTea.DeeplyDep.Models;

public class Solution
{
    public Node SolutionRoot { get; set; }
    public Dictionary<string, Node> AllNodes { get; } = new();
}