// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Sidekick
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSTextFieldCell artifactsURLText { get; set; }

		[Outlet]
		AppKit.NSTextFieldCell statusText { get; set; }

		[Action ("artifactURLChanged:")]
		partial void artifactURLChanged (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (artifactsURLText != null) {
				artifactsURLText.Dispose ();
				artifactsURLText = null;
			}

			if (statusText != null) {
				statusText.Dispose ();
				statusText = null;
			}
		}
	}
}
