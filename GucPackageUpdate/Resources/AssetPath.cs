using System.IO;

namespace GucPackageUpdate.Resources
{
    internal static class AssetPath
    {
        private static readonly string AssetsFolder = Path.Combine(AppInfo.AppDirectory, "Assets");
        private static readonly string RapFolder = Path.Combine(AssetsFolder, "rap");
        private const string RapExtension = ".rap";

        public static string GetRapPath(string contentId)
            => Path.Combine(RapFolder, contentId + RapExtension);
    }
}
