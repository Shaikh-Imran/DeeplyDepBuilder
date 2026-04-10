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
}