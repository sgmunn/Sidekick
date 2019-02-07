using System;
using System.IO;

namespace Sidekick
{
    public static class CacheFolder
    {
        public readonly static string CacheRoot = Path.Combine(Environment.GetEnvironmentVariable("HOME"), "Library", "Caches", "Xamarin Sidekick");

        public static string GetFolder(string name)
        {
            return Path.Combine(CacheRoot, name);
        }
    }
}
