// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace AutoLink
{
	[Register ("MainScreenController")]
	partial class MainScreenController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton btnGetStarted { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnLearnMore { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnGetStarted != null) {
				btnGetStarted.Dispose ();
				btnGetStarted = null;
			}

			if (btnLearnMore != null) {
				btnLearnMore.Dispose ();
				btnLearnMore = null;
			}
		}
	}
}
