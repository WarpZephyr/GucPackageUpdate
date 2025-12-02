using GucPackageUpdate.Cryptography;
using GucPackageUpdate.Logging;
using GucPackageUpdate.Resources;
using GucPackageUpdate.Text;
using libps3;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace GucPackageUpdate
{
    public class PackageList
    {
        private const string PkgListHeader = "__GUC_PKG_FILE_LIST__";
        private const string UsrDirName = "USRDIR";

        public string ContentId { get; set; }
        public bool Encrypted { get; set; }
        public List<string> Entries { get; set; }

        public PackageList(string contentId, bool encrypted, List<string> entries)
        {
            ContentId = contentId;
            Encrypted = encrypted;
            Entries = entries;
        }

        private static bool TryGetRap(string contentId, [NotNullWhen(true)] out byte[]? rap)
        {
            string rapPath = AssetPath.GetRapPath(contentId);
            if (!File.Exists(rapPath))
            {
                Log.WriteLine($"Error: Couldn't find rap file \"{contentId}.rap\" for decryption in assets.");
                rap = null;
                return false;
            }

            rap = File.ReadAllBytes(rapPath);
            if (rap.Length != 16)
            {
                Log.WriteLine($"Error: Rap file \"{contentId}.rap\" is not the expected length of {16}.");
                return false;
            }

            return true;
        }

        #region Read

        private static bool TryDecrypt(string path, byte[] bytes, string contentId, [NotNullWhen(true)] out byte[]? output)
        {
            if (!TryGetRap(contentId, out byte[]? rap))
            {
                output = null;
                return false;
            }

            output = EdatBuilder.Decrypt(bytes, DevKlicVault.GucDevKlic, rap, Path.GetFileName(path));
            return true;
        }

        public static bool TryFromFile(string path, [NotNullWhen(true)] out PackageList? packageList)
        {
            bool encrypted = false;
            string contentId = string.Empty;
            byte[] bytes = File.ReadAllBytes(path);
            if (NPD.Is(bytes))
            {
                encrypted = true;
                contentId = NPD.Read(bytes).ContentId;
                if (!TryDecrypt(path, bytes, contentId, out byte[]? output))
                {
                    packageList = null;
                    return false;
                }

                bytes = output;
            }

            var lines = LineParser.ReadAllLines(bytes);
            if (lines.Count < 0)
            {
                Log.WriteLine($"Error: plist \"{path}\" has no lines.");
                packageList = null;
                return false;
            }

            if (lines[0] != PkgListHeader)
            {
                Log.WriteLine($"Error: plist \"{path}\" does not have a valid header of \"{PkgListHeader}\".");
                packageList = null;
                return false;
            }

            lines.RemoveAt(0);
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                int lastCommaIndex = line.LastIndexOf(',');
                if (lastCommaIndex > -1)
                {
                    line = line[..lastCommaIndex];
                    lines[i] = line;
                }
            }

            packageList = new PackageList(contentId, encrypted, lines);
            return true;
        }

        #endregion

        #region Write

        public void Write(string gameRoot, string path)
        {
            if (!Directory.Exists(gameRoot))
            {
                Log.WriteLine($"Error: Game Root folder doesn't exist at: \"{gameRoot}\"");
                return;
            }

            if (Directory.Exists(path))
            {
                Log.WriteLine($"Error: A folder exists at the file repack path: \"{path}\"");
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine(PkgListHeader);

            for (int i = 0; i < Entries.Count; i++)
            {
                string entry = Entries[i];
                int usrDirIndex = entry.IndexOf(UsrDirName);
                if (usrDirIndex < 0)
                {
                    Log.WriteLine($"Warning: Couldn't find \"{UsrDirName}\" in path of \"{entry}\", skipping.");
                    continue;
                }

                string cleanedEntry = entry[(usrDirIndex + UsrDirName.Length)..];
                cleanedEntry = cleanedEntry.TrimStart('\\');
                cleanedEntry = cleanedEntry.TrimStart('/');
                cleanedEntry = cleanedEntry.TrimStart(Path.DirectorySeparatorChar);
                cleanedEntry = cleanedEntry.TrimStart(Path.AltDirectorySeparatorChar);
                cleanedEntry = cleanedEntry.Replace('\\', Path.DirectorySeparatorChar);
                cleanedEntry = cleanedEntry.Replace('/', Path.DirectorySeparatorChar);
                cleanedEntry = cleanedEntry.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

                string cleanedPath = Path.Combine(gameRoot, cleanedEntry);
                var fileInfo = new FileInfo(cleanedPath);
                if (!fileInfo.Exists)
                {
                    Log.WriteLine($"Warning: Couldn't find \"{cleanedPath}\", skipping.");
                    continue;
                }

                sb.AppendLine($"{entry},{fileInfo.Length}");
            }

            byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
            if (Encrypted)
            {
                if (ContentId == string.Empty)
                {
                    Log.WriteLine("Warning: ContentId is empty, skipping encryption.");
                }
                else if (!TryGetRap(ContentId, out byte[]? rap))
                {
                    Log.WriteLine($"Warning: Couldn't find rap for: \"{ContentId}\", skipping encryption.");
                }
                else
                {
                    byte[] edatBytes = EdatBuilder.Encrypt(bytes, DevKlicVault.GucDevKlic, rap, Path.GetFileName(path), ContentId);
                    File.WriteAllBytes(path, edatBytes);
                }
            }
            else
            {
                File.WriteAllBytes(path, bytes);
            }
        }

        #endregion
    }
}
