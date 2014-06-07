using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace AutoLink
{
	public partial class PasswordResetController : UIViewController
	{
		LoginScreenController loginScreen;
		//AppDelegate app = (AppDelegate)UIApplication.SharedApplication.Delegate;

		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public PasswordResetController ()
			: base (UserInterfaceIdiomIsPhone ? "PasswordResetController_iPhone" : "PasswordResetController_iPad", null)
		{
			loginScreen = new LoginScreenController ();
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

				this.PresentViewController(loginScreen,true,null);

			};
			//submit
			this.btnResetPwd.TouchUpInside += (sender, e) => {
				var screen = new ResetController();
				this.PresentViewController(screen,false,null);
			};


		}
	}
}

