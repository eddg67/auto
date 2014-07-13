using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using AutoLink.Models;
using System.Collections.Generic;

namespace AutoLink
{
	public partial class ResetController : UIViewController
	{
		AppDelegate app = (AppDelegate)UIApplication.SharedApplication.Delegate;

		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public ResetController ()
			: base (UserInterfaceIdiomIsPhone ? "ResetController_iPhone" : "ResetController_iPad", null)
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
			
			// Perform any additional setup after loading the view, typically from a nib.
			this.btnBackLogin.Layer.BorderWidth = 1.0f;
			this.btnBackLogin.Layer.BorderColor = UIColor.White.CGColor;
	
			this.btnBackLogin.TouchUpInside += (sender, e) => {
				app.ShowLogin();

			};


		}

	}
}

