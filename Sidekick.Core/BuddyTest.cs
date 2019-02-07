using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sidekick
{
    /// <summary>
    /// Provides the ability to buddy test the mpacks found in the artifacts json in the given application bundle
    /// </summary>
    public class BuddyTest
    {
        private readonly string artifactsFile;
        private readonly VisualStudioApp vsmacApp;

        public BuddyTest(string artifactsFile, VisualStudioApp vsmacApp)
        {
            this.artifactsFile = artifactsFile;
            this.vsmacApp = vsmacApp;
        }

        public async Task DownloadArtifactsAsync(IProgress<string> progress)
        {
            // download each of the mpacks
            var artifacts = JsonConvert.DeserializeObject<List<Artifact>>(File.ReadAllText(this.artifactsFile));
            // grab just the mpacks, but hard code excluding Xamarin.TestCloud.NUnit
            var mpacks = artifacts.Where(a => a.FileExtension.ToLower() == ".mpack" && !a.FileName.Contains("Xamarin.TestCloud.NUnit")).ToList();

            progress?.Report("downloading artifacts");
            var downloader = new Downloader(ConsoleLog.Log, "BuddyTest");
            var localPaths = await downloader.Download(mpacks.Select(m => m.Url).ToArray());

            foreach (var localFile in localPaths)
            {
                Console.WriteLine(localFile);

                try
                {
                    var addinName = MpackTools.GetAddinNameFromArtifactName(localFile);
                    progress?.Report($"instaling {addinName}");

                    // create a backup
                    vsmacApp.BackupAddin(addinName);
                    // remove it if it is still there - this might be a prior buddy test
                    MpackTools.RemoveAddinFromBundle(this.vsmacApp, addinName);

                    // add the addin to the app bundle
                    var addinDir = MpackTools.GetAddinPathInBundle(this.vsmacApp, addinName);
                    Directory.CreateDirectory(addinDir);
                    await MpackTools.UnzipAddinAsync(localFile, addinDir);
                } catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}
