using CodersTea.DeeplyDep.Models;
using CodersTea.DeeplyDep.Parsers;
using CodersTea.DeeplyDep.Utils;
using CommandLine;

namespace CodersTea.DeeplyDep;

public class Program
{
    public static void Main(string[] args)
    {
        var opts = Parser.Default.ParseArguments<CliOptions>(args).Value;
        if (opts.Verbose)
        {
            Logger.isTraceEnabled = true;
        }

        PlatformUtil.CurrentPlatform = Platform.Linux;
        var solutionParser = new SolutionParser();
        var solution = solutionParser.ParseSolution(opts.SolutionPath);

        solution.Dependencies.ForEach(n => Logger.Info(n.ToString()));
    }
}