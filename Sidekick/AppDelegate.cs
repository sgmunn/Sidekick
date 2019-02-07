using System;
using System.IO;
using AppKit;
using Foundation;

namespace Sidekick
{
    // TODO: clear cache command
    // TODO: global restore apps if we happen to fail for some reason


    [Register("AppDelegate")]
    public class AppDelegate : NSApplicationDelegate
    {
        public readonly ApplicationActionCenter actionCenter;

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
                    NSApplication.SharedApplication.ActivateIgnoringOtherApps(true);
                    var vc = NSApplication.SharedApplication.MainWindow.ContentViewController as ViewController;
                    vc.StartBuddyTest(filenames[0]);
                }
            }
        }

        [Export("application:openFile:")]
        public new bool OpenFile(NSApplication sender, string filename)
        {
            return true;
        }
    }
}
