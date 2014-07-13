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
	[Register ("ResetController")]
	partial class ResetController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton btnBackLogin { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnBackLogin != null) {
				btnBackLogin.Dispose ();
				btnBackLogin = null;
			}
		}
	}
}
