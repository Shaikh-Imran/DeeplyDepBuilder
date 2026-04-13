using CodersTea.DeeplyDep.Parsers;
using CodersTea.DeeplyDep.Utils;

namespace CodersTea.DeeplyDep;

public class Program
{
    public static void Main(string[] args)
    {
        PlatformUtil.CurrentPlatform = Platform.Linux;
        var solutionParser = new SolutionParser();
        var solution = solutionParser.ParseSolution("/Users/ishaikh/imran/project/dot-net-dependency-builder/mono-repo-example/MySolution/MySolution.sln");
        
        solution.Dependencies.ForEach(n => Logger.Info(n.ToString()));
    }
}