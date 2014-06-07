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
	[Register ("LoginScreenController")]
	partial class LoginScreenController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton btnForgotPassword { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnLogin { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnSignup { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField txtEmail { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField txtPassword { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnForgotPassword != null) {
				btnForgotPassword.Dispose ();
				btnForgotPassword = null;
			}

			if (btnLogin != null) {
				btnLogin.Dispose ();
				btnLogin = null;
			}

			if (btnSignup != null) {
				btnSignup.Dispose ();
				btnSignup = null;
			}

			if (txtEmail != null) {
				txtEmail.Dispose ();
				txtEmail = null;
			}

			if (txtPassword != null) {
				txtPassword.Dispose ();
				txtPassword = null;
			}
		}
	}
}
