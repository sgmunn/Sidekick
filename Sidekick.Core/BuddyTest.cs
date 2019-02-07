using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;

namespace Sidekick
{
    /// <summary>
    /// Provides the ability to buddy test the mpacks found in the artifacts json in the given application bundle
    /// </summary>
    public class BuddyTest
    {
        private readonly string artifactsFile;
        private readonly string applicationPath;

        public BuddyTest(string artifactsFile, string applicationPath)
        {
            this.artifactsFile = artifactsFile;
            this.applicationPath = applicationPath;
        }

        public async Task InitAsync()
        {
            // download each of the mpacks
            var artifacts = JsonConvert.DeserializeObject<List<Artifact>>(File.ReadAllText(this.artifactsFile));
            var mpacks = artifacts.Where(a => a.FileExtension.ToLower() == ".mpack").ToList();
            var downloader = new Downloader(ConsoleLog.Log, "BuddyTest");
            var localPaths = await downloader.Download(mpacks.Select(m => m.Url).ToArray());

            // given the mpacks that are downloaded, 
            // move them into the app bundle

            // we will want to save and restore the mpacks from the app bundle, but we can do that in a different class perhaps

            // save, create a link file in the cache, with a link file in the app bundle
            // delete mpacks
            // move into place
            // run the setup

            foreach (var localFile in localPaths)
            {
                var addinName = MpackTools.GetAddinNameFromArtifactName(localFile);
                MpackTools.RemoveAddinFromBundle(this.applicationPath, addinName);
                var addinDir = MpackTools.GetAddinPathInBundle(this.applicationPath, addinName);

                Directory.CreateDirectory(addinDir);
                MpackTools.UnzipAddin(localFile, addinDir);

            }

        }

    }

    public class Artifact
    {
        [JsonProperty (PropertyName = "url")]
        public string Url { get; set; }

        [JsonIgnore]
        public string FileName 
        {
            get
            {
                return Path.GetFileName(this.Url);
            }
        }

        [JsonIgnore]
        public string FileExtension
        {
            get
            {
                return Path.GetExtension(this.Url);
            }
        }
    }

    public static class MpackTools
    {
        public static string GetAddinNameFromArtifactName(string artifactName)
        {
            var mpackName = Path.GetFileNameWithoutExtension(artifactName);

            // we trim after the first '_'
            return mpackName.Substring(0, mpackName.IndexOf('_'));
        }

        public static void UnzipAddin(string localPath, string destinationPath)
        {
            var zipFile = new ICSharpCode.SharpZipLib.Zip.ZipFile(localPath);
            foreach (ZipEntry zipEntry in zipFile)
            {
                if (!zipEntry.IsFile)
                {
                    continue;
                }

                var entryFileName = zipEntry.Name;
                byte[] buffer = new byte[4096];
                var zipStream = zipFile.GetInputStream(zipEntry);

                var fullZipToPath = Path.Combine(destinationPath, entryFileName);
                var directoryName = Path.GetDirectoryName(fullZipToPath);
                if (directoryName.Length > 0)
                {
                    Directory.CreateDirectory(directoryName);
                }

                using (FileStream streamWriter = File.Create(fullZipToPath))
                {
                    StreamUtils.Copy(zipStream, streamWriter, buffer);
                }
            }
        }

        public static void RemoveAddinFromBundle(string appDir, string addinName)
        {
            var addinDir = GetAddinPathInBundle(appDir, addinName);
            if (Directory.Exists(addinDir))
            {
                Directory.Delete(addinDir, true);
            }
        }

        public static string GetAddinPathInBundle(string appDir, string addinName)
        {
            var addinsRoot = Path.Combine(appDir, "Contents", "Resources", "lib", "monodevelop", "Addins");
            return Path.Combine(addinsRoot, addinName);
        }
    }
}
