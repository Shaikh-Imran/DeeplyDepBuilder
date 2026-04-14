using CodersTea.DeeplyDep.Parsers;
using CodersTea.DeeplyDep.Utils;

namespace CodersTea.DeeplyDep.Services;

public class GraphBuilderService
{
    public void BuildGraph(string solutionPath)
    {
        var solutionParser = new SolutionParser();
        var solution = solutionParser.ParseSolution(solutionPath);

        solution.Dependencies.ForEach(n => Logger.Info(n.ToString()));
    }
}