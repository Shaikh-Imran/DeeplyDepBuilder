using System.Xml.Linq;
using CodersTea.DeeplyDep.Utils;

namespace CodersTea.DeeplyDep.Parsers;

public class ProjectParser
{
    public List<string> ParseAndGetDependentProjects(string projectPath)
    {
        var projectDoc = XDocument.Parse(File.ReadAllText(projectPath));
        Logger.Trace($"Parsing Project: {projectPath}");

        return projectDoc.Descendants("ItemGroup")
            .Elements("ProjectReference")
            .Select(node => node.Attribute("Include")?.Value)
            .Where(IsValidProjectReference)
            .Select(currentPath => FullPath(projectPath, currentPath!))
            .ToList();
    }

    private static string FullPath(string projectPath, string currentPath)
    {
        var combinedPath = Path.Combine(Path.GetDirectoryName(projectPath) ?? "", currentPath);
        var fullpath = Path.GetFullPath(combinedPath);
        return PlatformUtil.ToPlatformPath(fullpath);
    }

    private static bool IsValidProjectReference(string? project)
    {
        return project != null
               && (project.EndsWith(".csproj", StringComparison.InvariantCultureIgnoreCase)
                   || project.EndsWith(".fsproj", StringComparison.InvariantCultureIgnoreCase)
                   || project.EndsWith(".vbproj", StringComparison.InvariantCultureIgnoreCase));
    }
}