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
	[Register ("PasswordResetController")]
	partial class PasswordResetController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton btnResetPwd { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnTryAgain { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField txtEmail { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (txtEmail != null) {
				txtEmail.Dispose ();
				txtEmail = null;
			}

			if (btnResetPwd != null) {
				btnResetPwd.Dispose ();
				btnResetPwd = null;
			}

			if (btnTryAgain != null) {
				btnTryAgain.Dispose ();
				btnTryAgain = null;
			}
		}
	}
}
