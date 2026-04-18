using System.Xml.Linq;
using CodersTea.DeeplyDep.Utils;

namespace CodersTea.DeeplyDep.Parsers;

public class ProjectParser
{
    public List<string> ParseAndGetDependentProjects(string projectPath)
    {
        var projectDoc = XDocument.Parse(File.ReadAllText(projectPath));
        Logger.Trace($"Parsing Project: {projectPath}");

        var projectFolderPath = Path.GetDirectoryName(projectPath)!;
        return projectDoc.Descendants("ProjectReference")
            .Select(node => node.Attribute("Include")?.Value)
            .Where(IsValidProjectReference)
            .Select(currentPath => PathUtil.CombineAndGetFullPlatformPath(projectFolderPath, currentPath!))
            .Distinct()
            .ToList();
    }

    private static bool IsValidProjectReference(string? project)
    {
        return project != null
               && (project.EndsWith(".csproj", StringComparison.InvariantCultureIgnoreCase)
                   || project.EndsWith(".fsproj", StringComparison.InvariantCultureIgnoreCase)
                   || project.EndsWith(".vbproj", StringComparison.InvariantCultureIgnoreCase));
    }
}