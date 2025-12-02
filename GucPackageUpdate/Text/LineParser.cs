using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GucPackageUpdate.Text
{
    internal static class LineParser
    {
        public static List<string> ReadAllLines(byte[] bytes)
        {
            string? line;
            var lines = new List<string>();
            using var stream = new MemoryStream(bytes);
            using var sr = new StreamReader(stream, Encoding.UTF8);
            while ((line = sr.ReadLine()) != null)
                lines.Add(line);

            return lines;
        }
    }
}
