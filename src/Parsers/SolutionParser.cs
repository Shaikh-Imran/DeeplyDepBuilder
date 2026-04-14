using System.Text.RegularExpressions;
using CodersTea.DeeplyDep.Utils;

namespace CodersTea.DeeplyDep.Parsers;

public class SolutionParser
{
    public List<string> ParaseAndGetDependentProjects(string solutionPath)
    {
        Logger.Info($"Parsing Solution {Path.GetFileNameWithoutExtension(solutionPath)} with path: {solutionPath}");

        var content = File.ReadAllText(solutionPath);

        var solutionDir = Path.GetDirectoryName(solutionPath);

        var projectMatches = Regex.Matches(content,
            @"Project\(""\{[^}]+\}""\)\s*=\s*""([^""]+)"",\s*""([^""]+)""");

        Logger.Trace($"Found {projectMatches.Count} project references in solution file");

        var projectReferences = new List<string>();
        foreach (var projectMatchGroup in projectMatches.OfType<Match>())
        {
            var projectPath = projectMatchGroup.Groups[2].Value.Trim();

            if (!projectPath.EndsWith(".csproj", StringComparison.InvariantCulture)
                && !projectPath.EndsWith(".fsproj", StringComparison.InvariantCulture)
                && !projectPath.EndsWith(".vbproj", StringComparison.InvariantCulture)
               )
            {
                Logger.Trace($"Skipping non-project reference {projectPath}");
                continue;
            }

            var fullPath = PlatformUtil.ToPlatformPath(Path.GetFullPath(Path.Combine(solutionDir ?? "", projectPath)));

            if (!File.Exists(fullPath))
            {
                // TODO: need better handling, may be error log and exit. nio point in building graph if it going to fail anyway.
                Logger.Error($"Could not find project file {fullPath}. Conitinuing with next project reference.");
                continue;
            }

            projectReferences.Add(fullPath);
            Logger.Trace($"Found project {Path.GetFileNameWithoutExtension(fullPath)} with path {fullPath}");
        }

        return projectReferences;
    }
}