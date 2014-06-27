using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace AutoLink
{
	public partial class PasswordResetController : UIViewController
	{
		AppDelegate app = (AppDelegate)UIApplication.SharedApplication.Delegate;

		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public PasswordResetController ()
			: base (UserInterfaceIdiomIsPhone ? "PasswordResetController_iPhone" : "PasswordResetController_iPad", null)
		{
			//loginScreen = new LoginScreenController ();
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Perform any additional setup after loading the view, typically from a nib.
			this.btnResetPwd.Layer.BorderWidth = 1.0f;
			this.btnResetPwd.Layer.BorderColor = UIColor.White.CGColor;

			this.btnTryAgain.TouchUpInside += (sender, e) => {

				app.ShowLogin();

			};
			//submit
			this.btnResetPwd.TouchUpInside += (sender, e) => {
				app.ShowReset();
			};

			this.txtEmail.AttributedPlaceholder = new NSAttributedString ("   Email", null, UIColor.White, null, UIColor.White, null);
			this.txtEmail.BackgroundColor = UIColor.FromWhiteAlpha (.5f, .5f);



		}
	}
}

