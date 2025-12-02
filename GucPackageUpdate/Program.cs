using GucPackageUpdate.Logging;
using System.IO;

namespace GucPackageUpdate
{
    internal class Program
    {
        private const string UsrDirName = "USRDIR";

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                // Shortcut for easier use
                var config = AppConfig.Current;
                string packagelistPath = config.PackageListPath;
                if (Directory.Exists(packagelistPath))
                {
                    ProcessDirectory(packagelistPath);
                    Log.WriteLine("Finished.");
                    return;
                }

                Log.WriteLine($"This app has no GUI.\n" +
                    $"Please drag and drop the gundam unicorn packagelist folder onto the exe of this app to update the package lists.");
                return;
            }

            foreach (string arg in args)
            {
                if (Directory.Exists(arg))
                {
                    ProcessDirectory(arg);
                }
                else if (File.Exists(arg))
                {
                    string? directory = Path.GetDirectoryName(arg);
                    if (string.IsNullOrEmpty(directory))
                    {
                        Log.WriteLine($"Warning: Could not get folder name for file: \"{arg}\", skipping.");
                        continue;
                    }

                    int usrDirIndex = directory.IndexOf(UsrDirName);
                    if (usrDirIndex < 0)
                    {
                        Log.WriteLine($"Warning: Could not get USRDIR root of file folder: \"{directory}\", skipping.");
                        continue;
                    }

                    string gameRoot = directory[..(usrDirIndex + UsrDirName.Length)];
                    ProcessFile(gameRoot, arg);
                }
                else
                {
                    Log.WriteLine($"Warning: Not recognized as a file or folder: \"{arg}\", skipping.");
                }
            }

            Log.WriteLine("Finished.");
            Log.Pause();
        }

        static void ProcessFile(string gameRoot, string file)
        {
            if (!PackageList.TryFromFile(file, out PackageList? packageList))
            {
                return;
            }

            packageList.Write(gameRoot, file);
            Log.WriteLine($"Successfully parsed \"{Path.GetFileName(file)}\".");
        }

        static void ProcessDirectory(string directory)
        {
            int usrDirIndex = directory.IndexOf(UsrDirName);
            if (usrDirIndex < 0)
            {
                Log.WriteLine($"Warning: Could not get USRDIR root of folder: \"{directory}\", skipping.");
                return;
            }

            string gameRoot = directory[..(usrDirIndex + UsrDirName.Length)];
            foreach (string file in Directory.EnumerateFiles(directory, "*", SearchOption.TopDirectoryOnly))
            {
                ProcessFile(gameRoot, file);
            }
        }
    }
}
