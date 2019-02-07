using System;

using AppKit;
using Foundation;

namespace Sidekick
{
    public partial class ViewController : NSViewController, IProgress<string>
    {
        private NSTextField progressLabel;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.statusLabel.StringValue = "hello there";

            this.progressLabel = new NSTextField();
            this.progressLabel.TranslatesAutoresizingMaskIntoConstraints = false;
            this.progressLabel.Bezeled = false;
            this.progressLabel.DrawsBackground = false;
            this.progressLabel.Editable = false;
            this.progressLabel.Selectable = false;
            this.progressLabel.StringValue = "hello";
            //labelValue.Font = new NSFontManager().ConvertFont(labelValue.Font, NSFontTraitMask.Bold);
            this.View.AddSubview(this.progressLabel);

            this.progressLabel.Frame = new CoreGraphics.CGRect(0, 0, 100, 20);

        }

        public void Report(string value)
        {
            this.BeginInvokeOnMainThread(() =>
            {
                this.progressLabel.StringValue = value;
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
    }
}
