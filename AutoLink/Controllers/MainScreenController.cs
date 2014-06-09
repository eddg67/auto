using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace AutoLink
{
	public partial class MainScreenController : UIViewController
	{
		AppDelegate app = (AppDelegate)UIApplication.SharedApplication.Delegate;

		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public MainScreenController ()
			: base (UserInterfaceIdiomIsPhone ? "MainScreenController_iPhone" : "MainScreenController_iPad", null)
		{
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
			this.NavigationController.NavigationBarHidden = true;
			this.NavigationController.ToolbarHidden = true;
			this.HandleBtn ();
			
			// Perform any additional setup after loading the view, typically from a nib.
		}

		private void HandleBtn()
		{
			//tutorial screen
			this.btnLearnMore.TouchUpInside += (sender, e) => {

				app.ShowSearch();
			};
			//go to login screen
			this.btnGetStarted.TouchUpInside += (sender, e) => {
				app.ShowLogin();
								
			};
		}
	}
}

