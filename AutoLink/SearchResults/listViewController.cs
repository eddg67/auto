using System;
using System.Linq;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using AutoLink.Models;
using System.Collections.Generic;

namespace AutoLink
{
	public class listViewController : UITableViewController
	{
		public string searchId  { get; set; }
		public bool useBinId  { get; set; }

		public DetailViewController DetailViewController {
			get;
			set;
		}

		public listViewController (string id,bool bin) : base (UITableViewStyle.Grouped)
		{
			searchId = id;
			useBinId = bin;
			TableView.TableHeaderView = null;
			TableView.SeparatorInset = new UIEdgeInsets (0, 0, 0, 0);
			//TableView.ContentInset = new UIEdgeInsets (0, 5, 0, 5); 

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
			TableView.RowHeight = 0;
			//this.NavigationController.NavigationBarHidden = true;
			//TableView.ScrollEnabled = false; 
			// Register the TableView's data source
			if (useBinId) {
				TableView.Source = new listViewSource ("",searchId);
			} else {
				TableView.Source = new listViewSource (searchId);
			}

			UISwipeGestureRecognizer swipeGestureRecognizerRight = new UISwipeGestureRecognizer(HandleSwipeRight);
			swipeGestureRecognizerRight.Direction = UISwipeGestureRecognizerDirection.Right;
			this.View.AddGestureRecognizer(swipeGestureRecognizerRight);
		}

		private void HandleSwipeRight()
		{
			// load another view
			Console.WriteLine("Swipe right.");
		}
	}
}

