
using System.Runtime.InteropServices;

namespace CodersTea.DeeplyDep.Utils;


public static class PathUtil
{
    public static string ToPlatformPath(string path)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return path.Replace("/", "\\");
        }

        return path.Replace("\\", "/");
    }
    
    public static string CombineAndGetFullPlatformPath(string basepath, string filePath)
    {
        var combinedPath = Path.Combine(basepath, filePath);
        return Path.GetFullPath(ToPlatformPath(combinedPath));
    }
}