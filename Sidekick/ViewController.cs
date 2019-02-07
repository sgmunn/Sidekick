using System;
using System.Threading.Tasks;
using AppKit;
using Foundation;

namespace Sidekick
{
    public partial class ViewController : NSViewController, IProgress<string>
    {
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            
            this.statusText.StringValue = "hello!";
        }

        public void Report(string value)
        {
            this.BeginInvokeOnMainThread(() =>
            {
                this.statusText.StringValue = value;
            });
        }

        public override NSObject RepresentedObject
        {
            get
            {
                return base.RepresentedObject;
            }
            set
            {
                base.RepresentedObject = value;
                // Update the view, if already loaded.
            }
        }

        async partial void artifactURLChanged(Foundation.NSObject sender)
        {
            if (string.IsNullOrEmpty(this.artifactsURLText.StringValue))
            {
                return;
            }

            var artifactFile = await ArtifactDownloader.DownloadFromUrlAsync(this.artifactsURLText.StringValue);
            if (string.IsNullOrEmpty(artifactFile))
            {
                return;
            }

            await StartBuddyTest(artifactFile);
        }


        public async Task StartBuddyTest(string artifactFile)
        {
            // TODO: pass a cancellation token around to stop the process
            var progress = this as IProgress<string>;

            // TODO: clean this up
            var d = NSApplication.SharedApplication.Delegate as AppDelegate;
            var app = d.actionCenter.SelectedApp;
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
            await app.RegisterAddins(progress);
            progress?.Report("cleaned up");
        }
    }
}
