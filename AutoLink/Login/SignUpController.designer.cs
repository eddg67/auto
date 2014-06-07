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
	[Register ("SignUpController")]
	partial class SignUpController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton btnClose { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnSubmit { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField txtEmail { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField txtName { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField txtPassword { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField txtZip { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnClose != null) {
				btnClose.Dispose ();
				btnClose = null;
			}

			if (btnSubmit != null) {
				btnSubmit.Dispose ();
				btnSubmit = null;
			}

			if (txtEmail != null) {
				txtEmail.Dispose ();
				txtEmail = null;
			}

			if (txtName != null) {
				txtName.Dispose ();
				txtName = null;
			}

			if (txtPassword != null) {
				txtPassword.Dispose ();
				txtPassword = null;
			}

			if (txtZip != null) {
				txtZip.Dispose ();
				txtZip = null;
			}
		}
	}
}
