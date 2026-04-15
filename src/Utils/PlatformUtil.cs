namespace CodersTea.DeeplyDep.Utils;

public static class PlatformUtil
{
    public static Platform CurrentPlatform = Platform.Windows;

    public static string ToPlatformPath(string path)
    {
        if (CurrentPlatform == Platform.Windows)
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