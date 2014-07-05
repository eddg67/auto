
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using AutoLink.Models;

namespace AutoLink
{
	public partial class ImageViewController : UIViewController
	{
		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		Listing listing;
		int index;

		public ImageViewController (Listing _listing,int row)
			: base (UserInterfaceIdiomIsPhone ? "ImageViewController_iPhone" : "ImageViewController_iPad", null)
		{
			//image = img;
			listing = _listing;
			index = row;
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
			
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			NavigationController.NavigationBarHidden = false;
			NavigationController.ToolbarHidden = true;

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			NavigationController.NavigationBarHidden = false;
			NavigationController.ToolbarHidden = true;

			View.DownloadImageAsync (listing.images [index]).ContinueWith (
				(task) => InvokeOnMainThread (() => {
					if(task.IsFaulted){
						return;
					}

					if(task.Result != null){
						var res = task.Result;
						var siz = res.Size;
						ImageView.Image = res;
						var frame = View.Bounds;
						//frame.X = 50f;
						//frame.Height = siz.Height;

						//ImageView.Frame = frame;
					}
					//TextView.Text = listing.description;
				}));
					
			
			// Perform any additional setup after loading the view, typically from a nib.
	
		}
	}
}

