using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using AutoLink.Services;

namespace AutoLink
{
	public partial class RootController : UIViewController
	{
		AppDelegate app = (AppDelegate)UIApplication.SharedApplication.Delegate;
		LoginService loginService;


		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public RootController ()
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

			loginService = app.loginService;
			this.NavigationController.NavigationBarHidden = true;
			this.NavigationController.ToolbarHidden = true;

			//TODO make pass thru for logged in users
			if (loginService.IsLoggedIn()) {
				using (var screen = new SearchResultController ()) {
					this.NavigationController.PushViewController (screen, true);
				}

			} else {
				//splash screen
				using (var screen = new MainScreenController ()) {
					this.NavigationController.PushViewController (screen, true);
				}

			}
				
		}


	
	}
}

