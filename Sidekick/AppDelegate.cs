using System;
using System.IO;
using AppKit;
using Foundation;

namespace Sidekick
{
    [Register("AppDelegate")]
    public class AppDelegate : NSApplicationDelegate
    {
        private readonly ApplicationActionCenter actionCenter;

        public AppDelegate()
        {
            NSStatusBar bar = NSStatusBar.SystemStatusBar;
            NSStatusItem statusItem = bar.CreateStatusItem(NSStatusItemLength.Square);
            statusItem.Button.Image = NSImage.ImageNamed(NSImageName.StatusAvailable);

            this.actionCenter = new ApplicationActionCenter(statusItem);
        }

        public override void DidFinishLaunching(NSNotification notification)
        {
            this.actionCenter.Refresh();
        }

        public override void WillTerminate(NSNotification notification)
        {
            // Insert code here to tear down your application
        }

        [Export("application:openFiles:")]
        public new void OpenFiles(NSApplication sender, string[] filenames)
        {
            if (filenames.Length ==1)
            {
                var ext = Path.GetExtension(filenames[0]);
                if (string.Equals(ext, ".json", System.StringComparison.OrdinalIgnoreCase))
                {
                    Test(filenames[0]);
                }
            }
        }

        [Export("application:openFile:")]
        public new bool OpenFile(NSApplication sender, string filename)
        {
            return true;
        }

        async void Test(string artifactFile)
        {
            // TODO: pass a cancellation token around to stop the process
            var progress = NSApplication.SharedApplication.MainWindow.ContentViewController as IProgress<string>;
            NSApplication.SharedApplication.ActivateIgnoringOtherApps(true);

            // open the artifacts json and process it
            var app = this.actionCenter.SelectedApp;
            if (app == null)
            {
                progress?.Report("No application is selected");
                return;
            }

            var x = new BuddyTest(artifactFile, app);

            await x.DownloadArtifactsAsync(progress);
            await app.RegisterAddins(progress);

            progress?.Report("launching app and waiting for exit");
            await app.Start();

            app.RestoreAddins(progress);
        }
    }
}
