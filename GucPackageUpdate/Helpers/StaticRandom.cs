using System;

namespace GucPackageUpdate.Helpers
{
    internal class StaticRandom
    {
        [ThreadStatic]
        internal static Random Random;

        static StaticRandom()
        {
            Random = new Random();
        }
    }
}
