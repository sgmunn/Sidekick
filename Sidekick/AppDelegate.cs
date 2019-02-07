using System.IO;
using AppKit;
using Foundation;

namespace Sidekick
{
    [Register("AppDelegate")]
    public class AppDelegate : NSApplicationDelegate
    {
        public AppDelegate()
        {
        }

        public override void DidFinishLaunching(NSNotification notification)
        {
            // Insert code here to initialize your application
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
                    // open the artifacts json and process it
                    var x = new BuddyTest(filenames[0], $"/Applications/provisionator/Visual Studio.app");
                    x.InitAsync();
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
