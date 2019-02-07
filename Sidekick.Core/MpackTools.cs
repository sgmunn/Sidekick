using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Sidekick
{
    public static class MpackTools
    {
        public static string GetAddinNameFromArtifactName(string artifactName)
        {
            var mpackName = Path.GetFileNameWithoutExtension(artifactName);

            // we trim after the first '_'
            return mpackName.Substring(0, mpackName.IndexOf('_'));
        }

        public static Task UnzipAddinAsync(string localPath, string destinationPath)
        {
            return Task.Run(() =>
            {
                Process.Start("unzip", $"-o \"{localPath}\" -d \"{destinationPath}\"").WaitForExit();
            });
        }

        public static void RemoveAddinFromBundle(VisualStudioApp app, string addinName)
        {
            var addinDir = GetAddinPathInBundle(app, addinName);
            if (Directory.Exists(addinDir))
            {
                Directory.Delete(addinDir, true);
            }
        }

        public static string GetAddinPathInBundle(VisualStudioApp app, string addinName)
        {
            return Path.Combine(app.AddinsRoot, addinName);
        }
    }
}
