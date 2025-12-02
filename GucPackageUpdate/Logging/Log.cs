using System;
using System.Runtime.CompilerServices;

namespace GucPackageUpdate.Logging
{
    internal static class Log
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteLine()
            => Console.WriteLine();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteLine(string value)
            => Console.WriteLine(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(string value)
            => Console.Write(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Pause()
            => Console.ReadKey(true);
    }
}
