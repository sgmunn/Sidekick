using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sidekick
{
    public sealed class VisualStudioApp
    {
        private readonly static string BackupRoot = CacheFolder.GetFolder("AppBundleAddins");
        private string backupPath;
        private readonly string bundleLinkFile;

        public VisualStudioApp(string path)
        {
            this.AppPath = path;
            this.AddinsRoot = Path.Combine(path, "Contents", "Resources", "lib", "monodevelop", "Addins");

            this.bundleLinkFile = Path.Combine(this.AddinsRoot, "sidekick.txt");
        }

        public string AppPath { get; private set; }

        public string AddinsRoot { get; private set; }

        private bool BackupInitialised
        {
            get
            {
                return this.backupPath != null;
            }
        }

        public static IEnumerable<VisualStudioApp> FindApplications()
        {
            var files = new List<string>();

            // search /Applications, /Applications/provisionator
            if (Directory.Exists("/Applications/provisionator"))
            {
                files.AddRange(Directory.GetDirectories("/Applications/provisionator", "Visual Studio*.app"));
            }

            files.AddRange(Directory.GetDirectories("/Applications", "Visual Studio*.app"));

            foreach (var file in files.OrderByDescending(s => s))
            {
                var app = new VisualStudioApp(file);
                if (Directory.Exists(app.AddinsRoot))
                {
                    // TODO: check for backup, clean up if needed
                    yield return app;
                }
            }
        }

        public void BackupAddin(string addinName)
        {
            if (!this.BackupInitialised)
            {
                this.SetupBackup(true);
            }

            var addinBackupPath = Path.Combine(this.backupPath, addinName);
            if (Directory.Exists(addinBackupPath))
            {
                // already backed up
                return;
            }

            var addinPath = Path.Combine(this.AddinsRoot, addinName);
            if (Directory.Exists(addinPath))
            {
                Directory.Move(addinPath, addinBackupPath);
            }
        }

        public void RestoreAddins(IProgress<string> progress)
        {
            progress?.Report("restoring addins");

            if (!this.BackupInitialised)
            {
                this.SetupBackup(false);
            }

            if (!this.BackupInitialised)
            {
                return;
            }

            // iterate through the backed up addins and restore them
            var addinDirs = Directory.GetDirectories(this.backupPath);
            foreach (var addinDir in addinDirs)
            {
                var addinName = Path.GetFileName(addinDir);
                progress?.Report($"restoring {addinName}");

                var addinBundlePath = Path.Combine(this.AddinsRoot, addinName);
                if (Directory.Exists(addinBundlePath))
                {
                    Directory.Delete(addinBundlePath, true);
                }

                Directory.Move(addinDir, addinBundlePath);
            }

            if (File.Exists(this.bundleLinkFile))
            {
                File.Delete(this.bundleLinkFile);
            }

            Directory.Delete(this.backupPath, true);
            this.backupPath = null;
        }

        public async Task RegisterAddins(IProgress<string> progress)
        {
            progress?.Report("registering addins");

            var vstool = Path.Combine(this.AppPath, "Contents", "MacOS", "vstool");
            await Task.Run(() =>
            {
                try
                {
                    Process.Start(vstool, "setup reg-build").WaitForExit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });
        }

        public async Task Start()
        {
            var vsm = Path.Combine(this.AppPath, "Contents", "MacOS", "VisualStudio");
            await Task.Run(() =>
            {
                try
                {
                    Process.Start(vsm).WaitForExit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });
        }


        private void SetupBackup(bool createNew)
        {
            // look for a file in the app bundle that describes the cache dir for this instance
            // if not found, create a file that contains a guid we will cross reference 
            if (File.Exists(this.bundleLinkFile))
            {
                // we already set up a backup, where is it?
                var id = File.ReadAllText(this.bundleLinkFile);
                this.backupPath = Path.Combine(BackupRoot, id);

                // is it worth trying to validate it?
                //Directory.CreateDirectory(this.backupPath);
                //var linkFile = Path.Combine(this.backupPath, "sidekick.txt");
                //if (!File.Exists(linkFile))
                //{
                //    // the backup dir doesn't exist, 
                //    File.Delete(sidekickFile);
                //    this.backupPath = null;
                //} else
                //{
                //    var linkedAppDir = File.ReadAllText(linkFile);
                //    if (linkedAppDir != this.AppPath)
                //    {
                //        File.Delete(sidekickFile);
                //        Directory.Delete(this.backupPath, true);
                //        this.backupPath = null;
                //    }
                //}
            }

            if (createNew && !File.Exists(this.bundleLinkFile))
            {
                // we have not set up a backup location, let's create one
                SetupNewBackupLocation();
            }
        }

        private void SetupNewBackupLocation()
        {
            var id = Guid.NewGuid().ToString("N");

            this.backupPath = Path.Combine(BackupRoot, id);
            Directory.CreateDirectory(this.backupPath);
            var linkFile = Path.Combine(this.backupPath, "sidekick.txt");
            File.WriteAllText(linkFile, this.AppPath);

            File.WriteAllText(this.bundleLinkFile, id);
        }
    }
}
